using Campaign.Watch.Domain.Interfaces.Services.Read.Effpush;
using Campaign.Watch.Infra.Effpush.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Campaign.Watch.Infra.Effpush.Resolver
{
    public static class ResolverIoC
    {
        public static IServiceCollection AddEffpushRepository(this IServiceCollection services)
        {
            services.AddTransient<IEffpushReadService, EffpushReadService>();

            return services;
        }
    }
}
