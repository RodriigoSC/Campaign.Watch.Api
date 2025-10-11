using Campaign.Watch.Domain.Interfaces.Services.Read.Effmail;
using Campaign.Watch.Domain.Interfaces.Services.Read.Effsms;
using Campaign.Watch.Infra.Effsms.Services;
using Microsoft.Extensions.DependencyInjection;


namespace Campaign.Watch.Infra.Effsms.Resolver
{
    public static class ResolverIoC
    {
        public static IServiceCollection AddEffsmsRepository(this IServiceCollection services)
        {
            services.AddTransient<IEffsmsReadService, EffsmsReadService>();

            return services;
        }
    }
}
