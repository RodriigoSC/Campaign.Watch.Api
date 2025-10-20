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
            services.AddTransient<IClientApplication, ClientApplication>();
            services.AddTransient<ICampaignMonitoringApplication, CampaignMonitoringApplication>();            
            
            services.AddAutoMapper(typeof(ClientMapper)); 
            services.AddAutoMapper(typeof(CampaignProfile)); 
            services.AddAutoMapper(typeof(CampaignMonitoringProfile));
            services.AddAutoMapper(typeof(DiagnosticProfile));

            //services.AddAutoMapper(typeof(ResolverIoC).Assembly);

            return services;
        }
    }
}
