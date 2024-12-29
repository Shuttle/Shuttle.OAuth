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

        public OAuthBuilder AddOAuthOptions(string providerName, OAuthOptions oauthOptions)
        {
            Guard.AgainstNullOrEmptyString(providerName);
            Guard.AgainstNull(oauthOptions);

            Services.Configure<OAuthOptions>(providerName, options =>
            {
                options.AuthorizationUrl = oauthOptions.AuthorizationUrl;
                options.ClientId = oauthOptions.ClientId;
                options.ClientSecret = oauthOptions.ClientSecret;
                options.TokenUrl = oauthOptions.TokenUrl;
                options.TokenContentType = oauthOptions.TokenContentType;
                options.DataUrl = oauthOptions.DataUrl;
                options.DataAuthorizationScheme = oauthOptions.DataAuthorizationScheme;
                options.DataAccept = oauthOptions.DataAccept;
                options.CodeChallengeMethod = oauthOptions.CodeChallengeMethod;
                options.Scope = oauthOptions.Scope;
                options.EMailPropertyName = oauthOptions.EMailPropertyName;
            });

            return this;
        }
    }
}