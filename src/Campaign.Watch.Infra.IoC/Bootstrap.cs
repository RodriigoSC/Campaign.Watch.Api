using Campaign.Watch.Application.Resolver;
using Campaign.Watch.Infra.Data.Factories;
using Campaign.Watch.Infra.Data.Factories.Common;
using Campaign.Watch.Infra.Data.Resolver;
using DTM_Vault.Data;
using DTM_Vault.Data.Factory;
using DTM_Vault.Data.KeyValue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Campaign.Watch.Infra.IoC
{
    public static class Bootstrap
    {
       
        public static void StartIoC(IServiceCollection services, IConfiguration configuration)
        {
            var user_vault = ValidateIfNull(Environment.GetEnvironmentVariable("USER_VAULT"), "USER_VAULT");
            var pass_vault = ValidateIfNull(Environment.GetEnvironmentVariable("PASS_VAULT"), "PASS_VAULT");
            var conn_string_vault = ValidateIfNull(Environment.GetEnvironmentVariable("CONN_STRING_VAULT"), "CONN_STRING_VAULT");
            var environment = ValidateIfNull(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "ASPNETCORE_ENVIRONMENT");

            services.AddSingleton<IVaultFactory>(_ =>
                VaultFactory.CreateInstance(conn_string_vault, user_vault, pass_vault));
            services.AddTransient<IVaultService, VaultService>();

            services.AddSingleton<IMongoDbFactory>(sp =>
            {
                var vaultService = sp.GetRequiredService<IVaultService>();
                return new MongoDbFactory(vaultService, environment);
            });

            services.AddSingleton<IPersistenceMongoFactory>(sp =>
            {
                var mongoFactory = sp.GetRequiredService<IMongoDbFactory>();
                var vaultService = sp.GetRequiredService<IVaultService>();
                return new PersistenceMongoFactory(mongoFactory, vaultService, environment);
            });

            services.AddDataRepository();
            services.AddApplications();
        } 

        private static string ValidateIfNull(string? value, string name)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(name, $"A configuração '{name}' não pode ser nula ou vazia.");
            return value;
        }
    }
}

