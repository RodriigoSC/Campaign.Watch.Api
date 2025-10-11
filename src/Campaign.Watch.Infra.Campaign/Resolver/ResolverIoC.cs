using Campaign.Watch.Domain.Interfaces.Services.Read.Campaign;
using Campaign.Watch.Infra.Campaign.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Campaign.Watch.Infra.Campaign.Resolver
{
    /// <summary>
    /// Classe estática responsável por registrar as dependências da camada de Infra.Campaign.
    /// </summary>
    public static class ResolverIoC
    {
        /// <summary>
        /// Adiciona os serviços de leitura de campanha ao contêiner de injeção de dependência.
        /// </summary>
        /// <param name="services">A coleção de serviços para adicionar os registros.</param>
        /// <returns>A mesma coleção de serviços com os novos registros adicionados.</returns>
        public static IServiceCollection AddCampaignRepository(this IServiceCollection services)
        {
            // Registra a implementação do serviço de leitura de dados de campanha.
            services.AddTransient<ICampaignReadService, CampaignReadService>();

            return services;
        }
    }
}