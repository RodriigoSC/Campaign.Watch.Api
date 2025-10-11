using Campaign.Watch.Application.Interfaces.Campaign;
using Campaign.Watch.Application.Interfaces.Client;
using Campaign.Watch.Application.Interfaces.Read.Campaign;
using Campaign.Watch.Application.Interfaces.Worker;
using Campaign.Watch.Application.Mappers.Campaign;
using Campaign.Watch.Application.Mappers.Client;
using Campaign.Watch.Application.Services.Campaign;
using Campaign.Watch.Application.Services.Client;
using Campaign.Watch.Application.Services.Read.Campaign;
using Campaign.Watch.Application.Services.Worker;
using Microsoft.Extensions.DependencyInjection;

namespace Campaign.Watch.Application.Resolver
{
    public static class ResolverIoC
    {
        public static IServiceCollection AddApplications(this IServiceCollection services)
        {
            services.AddTransient<IClientApplication, ClientApplication>();
            services.AddTransient<ICampaignApplication, CampaignApplication>();

            services.AddTransient<ICampaignMonitorApplication, CampaignMonitorApplication>();
            services.AddTransient<ICampaignMonitorFlow, CampaignMonitorFlow>();
            services.AddScoped<ICampaignHealthCalculator, CampaignHealthCalculator>();
            services.AddScoped<ICampaignDataProcessor, CampaignDataProcessor>();

            services.AddAutoMapper(typeof(ClientMapper));
            services.AddAutoMapper(typeof(CampaignProfile));

            return services;
        }
    }
}
