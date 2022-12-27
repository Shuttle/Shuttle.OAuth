using Microsoft.Extensions.DependencyInjection;

namespace Shuttle.OAuth
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOAuth(this IServiceCollection services)
        {
            services.AddSingleton<IOAuthProviderService, OAuthProviderService>();

            return services;
        }
    }
}