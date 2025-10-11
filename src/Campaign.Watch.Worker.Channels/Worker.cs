using Campaign.Watch.Application.Interfaces.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Campaign.Watch.Worker.Channels
{
    /// <summary>
    /// Um serviço de background que monitora campanhas em ciclos periódicos.
    /// Este worker é responsável por orquestrar a execução do fluxo de monitoramento,
    /// implementando lógicas de retentativa, back-off em caso de falhas consecutivas e
    /// monitoramento de sua própria saúde operacional.
    /// </summary>
    public class Worker : BackgroundService
    {
        // Injeções de dependência e configurações
        private readonly ICampaignMonitorFlow _campaignMonitorFlow;
        private readonly ILogger<Worker> _logger;

        // Configurações do Worker
        private readonly TimeSpan _intervaloExecucao;
        private readonly bool _estaHabilitado;
        private readonly int _maxTentativas;
        private readonly TimeSpan _atrasoEntreTentativas;

        // Estado interno do Worker
        private DateTime _ultimaExecucaoComSucesso = DateTime.MinValue;
        private int _falhasConsecutivas = 0;

        /// <summary>
        /// Inicializa uma nova instância da classe Worker.
        /// </summary>
        /// <param name="fluxoMonitoramentoCampanha">O serviço que contém a lógica principal de monitoramento.</param>
        /// <param name="logger">A interface de log.</param>
        /// <param name="configuration">A configuração da aplicação para obter as definições do worker.</param>
        public Worker(ICampaignMonitorFlow fluxoMonitoramentoCampanha, ILogger<Worker> logger, IConfiguration configuration)
        {
            _campaignMonitorFlow = fluxoMonitoramentoCampanha;
            _logger = logger;

            // Carrega as configurações do appsettings.json
            _intervaloExecucao = TimeSpan.FromMinutes(configuration.GetValue("WorkerSettings:ExecutionIntervalMinutes", 5));
            _estaHabilitado = configuration.GetValue("WorkerSettings:Enabled", true);
            _maxTentativas = configuration.GetValue("WorkerSettings:MaxRetryAttempts", 3);
            _atrasoEntreTentativas = TimeSpan.FromSeconds(configuration.GetValue("WorkerSettings:RetryDelaySeconds", 30));

            _logger.LogInformation(
                "Worker de Monitoramento de Campanhas configurado. " +
                "Habilitado: {EstaHabilitado}, Intervalo: {IntervaloExecucao} min, " +
                "Máximo de Tentativas: {MaxTentativas}, Atraso entre Tentativas: {AtrasoEntreTentativas}s",
                _estaHabilitado, _intervaloExecucao.TotalMinutes, _maxTentativas, _atrasoEntreTentativas.TotalSeconds);
        }

        /// <summary>
        /// O método principal do serviço de background. Contém o loop que executa
        /// o monitoramento de campanhas enquanto o serviço estiver ativo.
        /// </summary>
        /// <param name="stoppingToken">O token que sinaliza quando o serviço deve ser interrompido.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_estaHabilitado)
            {
                _logger.LogWarning("O Worker está desabilitado nas configurações. A execução não será iniciada.");
                return;
            }

            _logger.LogInformation("Serviço de monitoramento de campanhas iniciado.");

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Atraso inicial

            while (!stoppingToken.IsCancellationRequested)
            {
                var inicioExecucao = DateTime.UtcNow;
                var executionId = Guid.NewGuid().ToString("N")[..12];

                // Anexa o ExecutionId a todos os logs dentro deste bloco para facilitar o rastreamento.
                using (_logger.BeginScope(new Dictionary<string, object> { ["ExecutionId"] = executionId }))
                {
                    try
                    {
                        _logger.LogInformation("Iniciando novo ciclo de monitoramento...");

                        await ExecutarComTentativasAsync(stoppingToken);

                        // Reseta o estado de falha após um ciclo bem-sucedido.
                        _ultimaExecucaoComSucesso = inicioExecucao;
                        _falhasConsecutivas = 0;

                        var duracao = DateTime.UtcNow - inicioExecucao;
                        _logger.LogInformation("Ciclo de monitoramento concluído com sucesso em {Duracao:F2}s.", duracao.TotalSeconds);

                        VerificarSaudeDoWorker();
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogWarning("O ciclo de monitoramento foi cancelado.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        _falhasConsecutivas++;
                        _logger.LogError(ex, "Erro crítico não tratado no ciclo de monitoramento. Falhas consecutivas: {FalhasConsecutivas}", _falhasConsecutivas);

                        // Estratégia de back-off: Aumenta o tempo de espera se ocorrerem muitas falhas seguidas.
                        if (_falhasConsecutivas >= 5)
                        {
                            var atrasoEstendido = TimeSpan.FromMinutes(_intervaloExecucao.TotalMinutes * 2);
                            _logger.LogWarning("Muitas falhas consecutivas. Aguardando um tempo maior: {AtrasoEstendido} min", atrasoEstendido.TotalMinutes);
                            await AtrasarProximoCicloAsync(atrasoEstendido, stoppingToken);
                        }
                    }
                }

                await AtrasarProximoCicloAsync(_intervaloExecucao, stoppingToken);
            }

            _logger.LogInformation("Serviço de monitoramento de campanhas finalizado.");
        }

        /// <summary>
        /// Tenta executar o fluxo principal de monitoramento com uma política de retentativas.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <exception cref="InvalidOperationException">Lançada se todas as tentativas de execução falharem.</exception>
        private async Task ExecutarComTentativasAsync(CancellationToken cancellationToken)
        {
            var tentativa = 1;
            Exception ultimaExcecao = null;

            while (tentativa <= _maxTentativas && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (tentativa > 1)
                        _logger.LogInformation("Iniciando tentativa {Tentativa}/{MaxTentativas}...", tentativa, _maxTentativas);
                    else
                        _logger.LogDebug("Executando o fluxo de monitoramento de campanhas.");

                    await _campaignMonitorFlow.MonitorarCampanhasAsync();
                    return; // Sucesso
                }
                catch (Exception ex)
                {
                    ultimaExcecao = ex;
                    _logger.LogWarning(ex, "Falha na tentativa {Tentativa}/{MaxTentativas}: {MensagemErro}", tentativa, _maxTentativas, ex.Message);

                    if (tentativa < _maxTentativas && !cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(_atrasoEntreTentativas, cancellationToken);
                    }
                    tentativa++;
                }
            }

            throw new InvalidOperationException($"Todas as {_maxTentativas} tentativas de execução falharam.", ultimaExcecao);
        }

        /// <summary>
        /// Realiza verificações periódicas da saúde do worker e emite logs de status ou alerta.
        /// </summary>
        private void VerificarSaudeDoWorker()
        {
            var tempoDesdeUltimoSucesso = DateTime.UtcNow - _ultimaExecucaoComSucesso;

            if (_ultimaExecucaoComSucesso != DateTime.MinValue && tempoDesdeUltimoSucesso > TimeSpan.FromHours(1))
            {
                _logger.LogWarning(
                    "ALERTA DE SAÚDE: Nenhuma execução foi concluída com sucesso há {TempoDesdeUltimoSucesso:F1} horas. Falhas consecutivas: {FalhasConsecutivas}",
                    tempoDesdeUltimoSucesso.TotalHours, _falhasConsecutivas);
            }
            else
            {
                // A cada 30 minutos, se tudo estiver bem, registra um log de "saúde".
                if (DateTime.UtcNow.Minute % 30 == 0)
                {
                    _logger.LogInformation("Status: Worker saudável. Último sucesso em: {UltimaExecucaoComSucesso:yyyy-MM-dd HH:mm:ss} UTC", _ultimaExecucaoComSucesso);
                }
            }
        }

        /// <summary>
        /// Aguarda por um determinado período de tempo de forma segura, respeitando o token de cancelamento.
        /// </summary>
        /// <param name="delay">O tempo de espera.</param>
        /// <param name="stoppingToken">O token de cancelamento.</param>
        private async Task AtrasarProximoCicloAsync(TimeSpan delay, CancellationToken stoppingToken)
        {
            try
            {
                if (delay > TimeSpan.Zero && !stoppingToken.IsCancellationRequested)
                {
                    _logger.LogDebug("Aguardando {Delay} para o próximo ciclo.", delay);
                    await Task.Delay(delay, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Espera para o próximo ciclo cancelada.");
            }
        }

        /// <summary>
        /// Chamado quando a aplicação está sendo finalizada para parar o serviço.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Recebido sinal para parar o serviço...");
            await base.StopAsync(cancellationToken);
            _logger.LogInformation("Serviço finalizado. Última execução com sucesso foi em: {UltimaExecucao}",
                 _ultimaExecucaoComSucesso == DateTime.MinValue ? "N/A" : _ultimaExecucaoComSucesso.ToString("yyyy-MM-dd HH:mm:ss UTC"));
        }
    }
}