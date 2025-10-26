using Campaign.Watch.Application.Interfaces.Campaign;
using Campaign.Watch.Application.Interfaces.Client;
using Campaign.Watch.Application.Mappers.Campaign;
using Campaign.Watch.Application.Mappers.Client;
using Campaign.Watch.Application.Services.Campaign;
using Campaign.Watch.Application.Services.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Campaign.Watch.Application.Resolver
{
    public static class ResolverIoC
    {
        public static IServiceCollection AddApplications(this IServiceCollection services)
        {
            // Serviço de Cliente
            services.AddTransient<IClientApplication, ClientApplication>();

            // Serviços de Campanha
            services.AddTransient<ICampaignApplication, CampaignApplication>();
            services.AddTransient<IExecutionApplication, ExecutionApplication>();
            services.AddTransient<IDiagnosticApplication, DiagnosticApplication>();
            services.AddTransient<IDashboardApplication, DashboardApplication>();

            // Mappers
            services.AddAutoMapper(typeof(ClientMapper));
            services.AddAutoMapper(typeof(CampaignMonitoringProfile));

            return services;
        }
    }
}
