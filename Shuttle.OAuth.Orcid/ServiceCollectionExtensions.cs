using Microsoft.Extensions.DependencyInjection;

namespace Shuttle.OAuth.Orcid
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOrcidOAuth(this IServiceCollection services)
        {
            services.AddSingleton<IOAuthProvider, OrcidOAuthProvider>();

            return services;
        }
    }
}