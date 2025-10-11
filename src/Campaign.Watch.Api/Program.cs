using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Campaign.Watch.Api
{
    /// <summary>
    /// A classe principal da aplicação API.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// O ponto de entrada principal para a aplicação.
        /// Este método configura e executa o host da aplicação web.
        /// </summary>
        /// <param name="args">Argumentos de linha de comando.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Cria e configura o construtor do host da aplicação (IHostBuilder) com os padrões para uma aplicação web.
        /// </summary>
        /// <param name="args">Argumentos de linha de comando passados para o construtor do host.</param>
        /// <returns>Uma instância de IHostBuilder configurada para usar a classe Startup.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}