using Campaign.Watch.Domain.Interfaces.Services.Read.Effwhatsapp;
using Campaign.Watch.Infra.Effwhatsapp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Campaign.Watch.Infra.Effwhatsapp.Resolver
{
    public static class ResolverIoC
    {
        public static IServiceCollection AddEffwhatsappRepository(this IServiceCollection services)
        {
            services.AddTransient<IEffwhatsappReadService, EffwhatsappReadService>();

            return services;
        }
    }
}
