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

        public OAuthBuilder AddOAuthProvider<T>() where T : class, IOAuthProvider
        {
            Services.AddSingleton<IOAuthProvider, T>();

            return this;
        }

        public OAuthBuilder AddOAuthOptions(string name, OAuthOptions oauthOptions)
        {
            Guard.AgainstNullOrEmptyString(name);
            Guard.AgainstNull(oauthOptions);

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