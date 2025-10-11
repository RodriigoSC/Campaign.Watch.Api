using Campaign.Watch.Application.Resolver;
using Campaign.Watch.Infra.Campaign.Factories;
using Campaign.Watch.Infra.Campaign.Resolver;
using Campaign.Watch.Infra.Data.Factories;
using Campaign.Watch.Infra.Data.Factories.Common;
using Campaign.Watch.Infra.Data.Resolver;
using Campaign.Watch.Infra.Effmail.Factories;
using Campaign.Watch.Infra.Effmail.Resolver;
using Campaign.Watch.Infra.Effpush.Factories;
using Campaign.Watch.Infra.Effpush.Resolver;
using Campaign.Watch.Infra.Effsms.Factories;
using Campaign.Watch.Infra.Effsms.Resolver;
using Campaign.Watch.Infra.Effwhatsapp.Factories;
using Campaign.Watch.Infra.Effwhatsapp.Resolver;
using DTM_Vault.Data;
using DTM_Vault.Data.Factory;
using DTM_Vault.Data.KeyValue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Campaign.Watch.Infra.IoC
{
    /// <summary>
    /// Classe central de inicialização para a configuração da injeção de dependência (IoC) da aplicação.
    /// </summary>
    public static class Bootstrap
    {
        /// <summary>
        /// Configura e registra todos os serviços, repositórios e fábricas no contêiner de injeção de dependência.
        /// </summary>
        /// <param name="services">A coleção de serviços para adicionar os registros.</param>
        /// <param name="configuration">A configuração da aplicação (não utilizada diretamente, mas mantida por convenção).</param>
        public static void StartIoC(IServiceCollection services, IConfiguration configuration)
        {
            // Valida e obtém as variáveis de ambiente necessárias para a conexão com o Vault e para definir o ambiente.
            var user_vault = ValidateIfNull(Environment.GetEnvironmentVariable("USER_VAULT"), "USER_VAULT");
            var pass_vault = ValidateIfNull(Environment.GetEnvironmentVariable("PASS_VAULT"), "PASS_VAULT");
            var conn_string_vault = ValidateIfNull(Environment.GetEnvironmentVariable("CONN_STRING_VAULT"), "CONN_STRING_VAULT");
            var environment = ValidateIfNull(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "ASPNETCORE_ENVIRONMENT");

            // Configura o serviço e a fábrica do Vault como Singleton.
            services.AddSingleton<IVaultFactory>(_ =>
                VaultFactory.CreateInstance(conn_string_vault, user_vault, pass_vault));
            services.AddTransient<IVaultService, VaultService>();

            // Registra a fábrica genérica de MongoDB como Singleton.
            services.AddSingleton<IMongoDbFactory>(sp =>
            {
                var vaultService = sp.GetRequiredService<IVaultService>();
                return new MongoDbFactory(vaultService, environment);
            });

            // Registra a fábrica específica para o banco de dados de persistência como Singleton.
            services.AddSingleton<IPersistenceMongoFactory>(sp =>
            {
                var mongoFactory = sp.GetRequiredService<IMongoDbFactory>();
                var vaultService = sp.GetRequiredService<IVaultService>();
                return new PersistenceMongoFactory(mongoFactory, vaultService, environment);
            });

            // Registra a fábrica específica para os bancos de dados de campanha como Singleton.
            services.AddSingleton<ICampaignMongoFactory>(sp =>
            {
                var mongoFactory = sp.GetRequiredService<IMongoDbFactory>();
                return new CampaignMongoFactory(mongoFactory);
            });

            // Registra a fábrica específica para os bancos de dados de emails como Singleton.
            services.AddSingleton<IEffmailMongoFactory>(sp =>
            {
                var mongoFactory = sp.GetRequiredService<IMongoDbFactory>();
                return new EffmailMongoFactory(mongoFactory);
            });

            // Registra a fábrica específica para os bancos de dados de sms como Singleton.
            services.AddSingleton<IEffsmsMongoFactory>(sp =>
            {
                var mongoFactory = sp.GetRequiredService<IMongoDbFactory>();
                return new EffsmsMongoFactory(mongoFactory);
            });

            // Registra a fábrica específica para os bancos de dados de push como Singleton.
            services.AddSingleton<IEffpushMongoFactory>(sp =>
            {
                var mongoFactory = sp.GetRequiredService<IMongoDbFactory>();
                return new EffpushMongoFactory(mongoFactory);
            });

            // Registra a fábrica específica para os bancos de dados de whatsapp como Singleton.
            services.AddSingleton<IEffwhatsappMongoFactory>(sp =>
            {
                var mongoFactory = sp.GetRequiredService<IMongoDbFactory>();
                return new EffwhatsappMongoFactory(mongoFactory);
            });

            // Chama os métodos de extensão de outras camadas para registrar suas respectivas dependências.
            services.AddDataRepository();

            services.AddCampaignRepository();

            services.AddEffmailRepository();
            services.AddEffsmsRepository();
            services.AddEffpushRepository();
            services.AddEffwhatsappRepository();

            services.AddApplications();
        }

        /// <summary>
        /// Valida se um valor de string é nulo ou vazio.
        /// </summary>
        /// <param name="value">O valor a ser validado.</param>
        /// <param name="name">O nome da variável ou configuração, usado na mensagem de exceção.</param>
        /// <returns>O valor original se não for nulo ou vazio.</returns>
        /// <exception cref="ArgumentNullException">Lançada se o valor for nulo ou vazio.</exception>
        private static string ValidateIfNull(string? value, string name)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(name, $"A configuração '{name}' não pode ser nula ou vazia.");
            return value;
        }
    }
}

