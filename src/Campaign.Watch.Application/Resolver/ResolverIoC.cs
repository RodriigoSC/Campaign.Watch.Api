using Campaign.Watch.Application.Interfaces.Alerts;
using Campaign.Watch.Application.Interfaces.Campaign;
using Campaign.Watch.Application.Interfaces.Client;
using Campaign.Watch.Application.Interfaces.Users;
using Campaign.Watch.Application.Mappers.Alerts;
using Campaign.Watch.Application.Mappers.Campaign;
using Campaign.Watch.Application.Mappers.Client;
using Campaign.Watch.Application.Mappers.Users;
using Campaign.Watch.Application.Services.Alerts;
using Campaign.Watch.Application.Services.Campaign;
using Campaign.Watch.Application.Services.Client;
using Campaign.Watch.Application.Services.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Campaign.Watch.Application.Resolver
{
    public static class ResolverIoC
    {
        public static IServiceCollection AddApplications(this IServiceCollection services)
        {
            // Serviço de Users e Clients
            services.AddTransient<IUserApplication, UserApplication>();
            services.AddTransient<IClientApplication, ClientApplication>();

            // Serviços de Campanha
            services.AddTransient<ICampaignApplication, CampaignApplication>();
            services.AddTransient<IExecutionApplication, ExecutionApplication>();
            services.AddTransient<IDiagnosticApplication, DiagnosticApplication>();
            services.AddTransient<IDashboardApplication, DashboardApplication>();

            services.AddTransient<IAlertApplication, AlertApplication>();

            // Mappers
            services.AddAutoMapper(typeof(UserProfile));
            services.AddAutoMapper(typeof(ClientProfile));
            services.AddAutoMapper(typeof(CampaignMonitoringProfile));
            services.AddAutoMapper(typeof(AlertProfile));

            return services;
        }
    }
}
