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

        public OAuthBuilder AddOAuthOptions<T>(string providerName, OAuthOptions oauthOptions) where T : class, IOAuthService
        {
            Guard.AgainstNullOrEmptyString(providerName);
            Guard.AgainstNull(oauthOptions);

            Services.AddSingleton<IOAuthService, T>();

            Services.Configure<OAuthOptions>(providerName, options =>
            {
                options.ClientId = oauthOptions.ClientId;
                options.ClientSecret = oauthOptions.ClientSecret;
                options.TokenUrl = oauthOptions.TokenUrl;
                options.TokenContentType = oauthOptions.TokenContentType;
                options.DataUrl = oauthOptions.DataUrl;
                options.DataAuthorization = oauthOptions.DataAuthorization;
                options.DataAccept = oauthOptions.DataAccept;
            });

            return this;
        }
    }
}