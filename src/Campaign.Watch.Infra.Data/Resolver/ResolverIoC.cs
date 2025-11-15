using Campaign.Watch.Domain.Interfaces.Repositories.Alerts;
using Campaign.Watch.Domain.Interfaces.Repositories.Campaign;
using Campaign.Watch.Domain.Interfaces.Repositories.Client;
using Campaign.Watch.Domain.Interfaces.Repositories.Users;
using Campaign.Watch.Domain.Interfaces.Services.Campaign;
using Campaign.Watch.Domain.Interfaces.Services.Client;
using Campaign.Watch.Domain.Interfaces.Services.Users;
using Campaign.Watch.Infra.Data.Repository.Alerts;
using Campaign.Watch.Infra.Data.Repository.Campaigns;
using Campaign.Watch.Infra.Data.Repository.Clients;
using Campaign.Watch.Infra.Data.Repository.Users;
using Campaign.Watch.Infra.Data.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Campaign.Watch.Infra.Data.Resolver
{
    /// <summary>
    /// Classe estática responsável por registrar as dependências da camada de Infra.Data (Resolver Inversion of Control).
    /// </summary>
    public static class ResolverIoC
    {
        /// <summary>
        /// Adiciona os repositórios e serviços da camada de dados ao contêiner de injeção de dependência.
        /// </summary>
        /// <param name="services">A coleção de serviços para adicionar os registros.</param>
        /// <returns>A mesma coleção de serviços com os novos registros adicionados para permitir chamadas encadeadas.</returns>
        public static IServiceCollection AddDataRepository(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserService, UserService>();

            // Registra as dependências de Cliente (Repositório e Serviço)
            services.AddTransient<IClientRepository, ClientRepository>();
            services.AddTransient<IClientService, ClientService>();

            // Registra as dependências de Campanha (Repositório e Serviço)
            services.AddTransient<ICampaignRepository, CampaignRepository>();
            services.AddTransient<ICampaignService, CampaignService>();

            services.AddTransient<IExecutionRepository, ExecutionRepository>();

            services.AddTransient<IAlertConfigurationRepository, AlertConfigurationRepository>();
            services.AddTransient<IAlertHistoryRepository, AlertHistoryRepository>();


            return services;
        }
    }
}