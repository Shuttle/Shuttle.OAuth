using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.OAuth;

public static class OAuthBuilderExtensions
{
    extension(OAuthBuilder oauthBuilder)
    {
        public OAuthBuilder AddInMemoryOAuthGrantRepository()
        {
            Guard.AgainstNull(oauthBuilder);

            oauthBuilder.Services.AddSingleton<IOAuthGrantRepository, InMemoryOAuthGrantRepository>();

            return oauthBuilder;
        }
    }
}