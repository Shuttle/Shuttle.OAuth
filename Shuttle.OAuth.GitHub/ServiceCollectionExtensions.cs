using Microsoft.Extensions.DependencyInjection;

namespace Shuttle.OAuth.GitHub
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGitHubOAuth(this IServiceCollection services)
        {
            services.AddSingleton<IOAuthProvider, GitHubOAuthProvider>();

            return services;
        }
    }
}