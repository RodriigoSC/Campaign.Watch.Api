using Campaign.Watch.Domain.Entities.Common;
using Campaign.Watch.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Campaign.Watch.Domain.Entities.Campaign
{
    /// <summary>
    /// Representa a entidade principal de uma campanha no sistema de monitoramento.
    /// Herda propriedades comuns de CommonEntity.
    /// </summary>
    public class CampaignEntity : CommonEntity
    {
        /// <summary>
        /// Nome do cliente ao qual a campanha pertence.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// ID da campanha no sistema de origem.
        /// </summary>
        public string IdCampaign { get; set; }

        /// <summary>
        /// ID numérico sequencial da campanha.
        /// </summary>
        public long NumberId { get; set; }

        /// <summary>
        /// Nome da campanha.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tipo da campanha (ex: Pontual, Recorrente).
        /// </summary>
        public CampaignType CampaignType { get; set; }

        /// <summary>
        /// Descrição detalhada da campanha.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// ID do projeto ao qual a campanha está associada.
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// Indica se a campanha está ativa.
        /// </summary>
        /// 
        [BsonRepresentation(BsonType.Boolean)]
        public bool IsActive { get; set; }

        /// <summary>
        /// Data e hora de criação da campanha na origem.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Data e hora da última modificação da campanha na origem.
        /// </summary>
        public DateTime ModifiedAt { get; set; }

        /// <summary>
        /// Status atual da campanha na origem (ex: Executando, Concluída).
        /// </summary>
        public CampaignStatus StatusCampaign { get; set; }

        /// <summary>
        /// Status da campanha no sistema de monitoramento (ex: Em Andamento, Atrasada).
        /// </summary>
        public MonitoringStatus MonitoringStatus { get; set; }

        /// <summary>
        /// Data e hora previstas para a próxima verificação ou execução do monitoramento.
        /// </summary>
        public DateTime? NextExecutionMonitoring { get; set; }

        /// <summary>
        /// Data e hora da última verificação realizada pelo monitoramento.
        /// </summary>
        public DateTime? LastCheckMonitoring { get; set; }

        /// <summary>
        /// Agrupamento para flags de saúde do monitoramento.
        /// </summary>
        public MonitoringHealthStatus HealthStatus { get; set; }

        /// <summary>
        /// Flag que indica se a campanha foi logicamente deletada.
        /// </summary>
        /// 
        [BsonRepresentation(BsonType.Boolean)]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Flag que indica se a campanha foi restaurada após ser deletada.
        /// </summary>
        /// 
        [BsonRepresentation(BsonType.Boolean)]
        public bool IsRestored { get; set; }

        /// <summary>
        /// Detalhes do agendamento da campanha.
        /// </summary>
        public Scheduler Scheduler { get; set; }

        /// <summary>
        /// Lista das execuções históricas da campanha.
        /// </summary>
        public List<Execution> Executions { get; set; }
    }

    /// <summary>
    /// Objeto para agrupar as flags de saúde e problemas do monitoramento.
    /// </summary>
    public class MonitoringHealthStatus
    {
        /// <summary>
        /// Indica se a campanha foi totalmente processada e verificada, incluindo todas as suas integrações.
        /// </summary>
        /// 
        [BsonRepresentation(BsonType.Boolean)]

        public bool IsFullyVerified { get; set; }

        /// <summary>
        /// Flag que aponta se existe alguma execução que deveria ter rodado (baseado no agendamento) mas ainda não foi encontrada na origem.
        /// </summary>
        /// 
        [BsonRepresentation(BsonType.Boolean)]

        public bool HasPendingExecution { get; set; }

        /// <summary>
        /// Flag geral que indica se há algum erro de integração em qualquer uma das execuções.
        /// </summary>
        /// 
        [BsonRepresentation(BsonType.Boolean)]

        public bool HasIntegrationErrors { get; set; }

        /// <summary>
        /// Armazena o ID da última execução que apresentou algum problema, facilitando a depuração.
        /// </summary>
        public string LastExecutionWithIssueId { get; set; }

        /// <summary>
        /// Armazena a última mensagem de erro ou nota importante do monitoramento.
        /// </summary>
        public string LastMessage { get; set; }
    }

    /// <summary>
    /// Contém as informações de agendamento de uma campanha.
    /// </summary>
    public class Scheduler
    {
        /// <summary>
        /// Data e hora de início do agendamento.
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// Data e hora de término do agendamento (opcional).
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// Indica se a campanha é recorrente.
        /// </summary>
        /// 
        [BsonRepresentation(BsonType.Boolean)]

        public bool IsRecurrent { get; set; }

        /// <summary>
        /// Expressão Crontab para definir a recorrência.
        /// </summary>
        public string Crontab { get; set; }
    }

    /// <summary>
    /// Representa uma única execução (um disparo) de uma campanha.
    /// </summary>
    public class Execution
    {
        /// <summary>
        /// ID único da execução.
        /// </summary>
        public string ExecutionId { get; set; }

        /// <summary>
        /// Nome da campanha associada a esta execução.
        /// </summary>
        public string CampaignName { get; set; }

        /// <summary>
        /// Status geral da execução (ex: Running, Completed).
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Data e hora de início da execução.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Data e hora de término da execução (nulo se ainda estiver em andamento).
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Latência em segundos entre a execução esperada e o início real.
        /// Um valor alto pode indicar lentidão na infraestrutura de origem.
        /// </summary>
        public double StartLatencyInSeconds { get; set; }

        /// <summary>
        /// Duração total da execução em segundos, calculada do início ao fim.
        /// </summary>
        public double? TotalDurationInSeconds { get; set; }

        /// <summary>
        /// Indica se todos os steps de canal dentro desta execução foram verificados e tiveram seus dados de integração populados pelo worker.
        /// </summary>
        /// 
        [BsonRepresentation(BsonType.Boolean)]

        public bool IsFullyVerifiedByMonitoring { get; set; }

        /// <summary>
        /// Indica se algum erro foi detectado durante o monitoramento desta execução específica (ex: falha ao buscar dados de integração).
        /// </summary>
        /// 
        [BsonRepresentation(BsonType.Boolean)]

        public bool HasMonitoringErrors { get; set; }

        /// <summary>
        /// Lista de etapas (workflows) que compõem a execução.
        /// </summary>
        public List<Workflows> Steps { get; set; }
    }

    /// <summary>
    /// Representa uma etapa individual dentro do fluxo de uma execução de campanha.
    /// </summary>
    public class Workflows
    {
        /// <summary>
        /// ID da etapa do workflow na origem.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Nome da etapa do workflow.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tipo da etapa (ex: Filter, Channel, Wait).
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Status da etapa (ex: Completed, Running).
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Número total de usuários que passaram por esta etapa.
        /// </summary>
        public long TotalUser { get; set; }

        /// <summary>
        /// Tempo total de execução da etapa em milissegundos.
        /// </summary>
        public long TotalExecutionTime { get; set; }

        /// <summary>
        /// Objeto que armazena informações de erro, se houver.
        /// </summary>
        public object Error { get; set; }

        /// <summary>
        /// Nome do canal extraído dos dados da execução, se o tipo for 'Channel'.
        /// Ex: "EffectiveMail", "EffectiveSms".
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// Armazena notas ou mensagens de erro específicas do worker para este step.
        /// </summary>
        public string MonitoringNotes { get; set; }

        /// <summary>
        /// Dados específicos da integração de canal (Email, SMS, Push), se aplicável.
        /// </summary>
        public IntegrationDataBase IntegrationData { get; set; }
    }

    /// <summary>
    /// Classe base abstrata para dados de integração de diferentes canais.
    /// Utiliza BsonKnownTypes para suportar polimorfismo no MongoDB.
    /// </summary>
    [BsonKnownTypes(typeof(EmailIntegrationData), typeof(SmsIntegrationData), typeof(PushIntegrationData), typeof(WhatsAppIntegrationData))]
    public abstract class IntegrationDataBase
    {

        public string? Raw { get; set; }
        /// <summary>
        /// Nome do canal de comunicação (ex: E-mail, SMS).
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// Status do processo de integração.
        /// </summary>
        public string IntegrationStatus { get; set; }
    }

    /// <summary>
    /// Dados de integração específicos para o canal de E-mail.
    /// </summary>
    public class EmailIntegrationData : IntegrationDataBase
    {
        /// <summary>
        /// ID do template de e-mail utilizado.
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// Informações sobre o arquivo de leads processado.
        /// </summary>
        public FileInfoData File { get; set; }

        /// <summary>
        /// Estatísticas sobre o processamento dos leads.
        /// </summary>
        public LeadsData Leads { get; set; }
    }

    /// <summary>
    /// Dados de integração específicos para o canal de SMS.
    /// </summary>
    public class SmsIntegrationData : IntegrationDataBase
    {
        /// <summary>
        /// ID do template de e-mail utilizado.
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// Informações sobre o arquivo de leads processado.
        /// </summary>
        public FileInfoData File { get; set; }

        /// <summary>
        /// Estatísticas sobre o processamento dos leads.
        /// </summary>
        public LeadsData Leads { get; set; }
    }

    /// <summary>
    /// Dados de integração específicos para o canal de Push Notification.
    /// </summary>
    public class PushIntegrationData : IntegrationDataBase
    {
        /// <summary>
        /// ID do template de e-mail utilizado.
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// Informações sobre o arquivo de leads processado.
        /// </summary>
        public FileInfoData File { get; set; }

        /// <summary>
        /// Estatísticas sobre o processamento dos leads.
        /// </summary>
        public LeadsData Leads { get; set; }
    }

    /// <summary>
    /// Dados de integração específicos para o canal de Push Notification.
    /// </summary>
    public class WhatsAppIntegrationData : IntegrationDataBase
    {
        /// <summary>
        /// ID do template de e-mail utilizado.
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// Informações sobre o arquivo de leads processado.
        /// </summary>
        public FileInfoData File { get; set; }

        /// <summary>
        /// Estatísticas sobre o processamento dos leads.
        /// </summary>
        public LeadsData Leads { get; set; }
    }

    /// <summary>
    /// Armazena metadados sobre um arquivo processado.
    /// </summary>
    public class FileInfoData
    {
        /// <summary>
        /// Nome do arquivo.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Data e hora de início do processamento do arquivo.
        /// </summary>
        public DateTime? StartedAt { get; set; }

        /// <summary>
        /// Data e hora de finalização do processamento do arquivo.
        /// </summary>
        public DateTime? FinishedAt { get; set; }

        /// <summary>
        /// Número total de registros no arquivo.
        /// </summary>
        public long Total { get; set; }
    }

    /// <summary>
    /// Armazena estatísticas do processamento de leads de uma campanha.
    /// </summary>
    public class LeadsData
    {
        /// <summary>
        /// Número de leads bloqueados.
        /// </summary>
        public int? Blocked { get; set; }

        /// <summary>
        /// Número de leads removidos por duplicação.
        /// </summary>
        public int? Deduplication { get; set; }

        /// <summary>
        /// Número de leads que resultaram em erro.
        /// </summary>
        public int? Error { get; set; }

        /// <summary>
        /// Número de leads que solicitaram opt-out.
        /// </summary>
        public int? Optout { get; set; }

        /// <summary>
        // Número de leads processados com sucesso.
        /// </summary>
        public int? Success { get; set; }
    }
}