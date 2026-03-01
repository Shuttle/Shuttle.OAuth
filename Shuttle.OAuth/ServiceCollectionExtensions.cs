using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.OAuth;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddOAuth(Action<OAuthBuilder>? builder = null)
        {
            Guard.AgainstNull(services);

            var oauthBuilder = new OAuthBuilder(services);

            builder?.Invoke(oauthBuilder);

            services.Configure<OAuthOptions>(options =>
            {
                foreach (var kvp in oauthBuilder.Options)
                {
                    options[kvp.Key] = kvp.Value;
                }
            });

            services.AddSingleton<IValidateOptions<OAuthOptions>, OAuthOptionsValidator>();
            services.AddSingleton<IOAuthService, OAuthService>();
            services.AddSingleton<ICodeChallenge, S256CodeChallenge>();

            services.AddHttpClient("Shuttle.OAuth");

            return services;
        }
    }
}