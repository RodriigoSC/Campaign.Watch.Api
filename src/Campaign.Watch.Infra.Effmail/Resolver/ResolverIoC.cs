using Campaign.Watch.Domain.Interfaces.Services.Read.Effmail;
using Campaign.Watch.Infra.Effmail.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Campaign.Watch.Infra.Effmail.Resolver
{
    public static class ResolverIoC
    {
        public static IServiceCollection AddEffmailRepository(this IServiceCollection services)
        {
            services.AddTransient<IEffmailReadService, EffmailReadService>();

            return services;
        }
    }
}
