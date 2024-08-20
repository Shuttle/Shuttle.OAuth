using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.OAuth
{
    public class OAuthBuilder
    {
        public OAuthBuilder(IServiceCollection services)
        {
            Guard.AgainstNull(services, nameof(services));

            Services = services;
        }

        public IServiceCollection Services { get; }

        public OAuthBuilder AddOAuthProvider<T>(string name, OAuthOptions oauthOptions) where T : class, IOAuthProvider
        {
            Guard.AgainstNullOrEmptyString(name);
            Guard.AgainstNull(oauthOptions);

            Services.AddSingleton<IOAuthProvider, T>();

            Services.Configure<OAuthOptions>(name, options =>
            {
                options.ClientId = oauthOptions.ClientId;
                options.ClientSecret = oauthOptions.ClientSecret;
                options.TokenUrl = oauthOptions.TokenUrl;
                options.DataUrl = oauthOptions.DataUrl;
            });

            return this;
        }
    }
}