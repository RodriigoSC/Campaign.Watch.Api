using Campaign.Watch.Infra.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Campaign.Watch.Worker.Channels
{
    /// <summary>
    /// A classe principal da aplicação Worker Service.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// O ponto de entrada principal para a aplicação.
        /// </summary>
        /// <param name="args">Argumentos de linha de comando.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Cria e configura o construtor do host da aplicação (IHostBuilder).
        /// </summary>
        /// <param name="args">Argumentos de linha de comando passados para o construtor do host.</param>
        /// <returns>Uma instância de IHostBuilder configurada.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                // Configura as fontes de configuração da aplicação.
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = ValidateIfNull(hostingContext.HostingEnvironment.EnvironmentName, "ASPNETCORE_ENVIRONMENT");

                    config.SetBasePath(AppContext.BaseDirectory)
                          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                          .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
                          .AddEnvironmentVariables();
                })
                // Configura os serviços da aplicação e a injeção de dependência.
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                    // Registra a configuração como um Singleton para ser acessível em toda a aplicação.
                    services.AddSingleton(configuration);

                    // Adiciona a classe Worker principal como Scoped.
                    services.AddScoped<Worker>();

                    // Chama o método de bootstrap para configurar a injeção de dependência das outras camadas.
                    Bootstrap.StartIoC(services, configuration);

                    // Registra a classe Worker como um serviço hospedado (Hosted Service) para ser executado em background.
                    services.AddHostedService<Worker>();
                });

        /// <summary>
        /// Valida se um valor de string é nulo ou vazio.
        /// </summary>
        /// <param name="value">O valor a ser validado.</param>
        /// <param name="name">O nome da variável, usado na mensagem de exceção.</param>
        /// <returns>O valor original se não for nulo ou vazio.</returns>
        /// <exception cref="ArgumentNullException">Lançada se o valor for nulo ou vazio.</exception>
        private static string ValidateIfNull(string? value, string? name)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException($"{name} cannot be null");

            return value;
        }
    }
}