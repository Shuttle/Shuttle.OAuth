using Microsoft.Extensions.DependencyInjection;
using Shuttle.Contract;

namespace Shuttle.OAuth;

public static class OAuthBuilderExtensions
{
    extension(OAuthBuilder oauthBuilder)
    {
        public OAuthBuilder UseInMemoryOAuthGrantRepository()
        {
            Guard.AgainstNull(oauthBuilder);

            oauthBuilder.Services.AddSingleton<IOAuthGrantRepository, InMemoryOAuthGrantRepository>();

            return oauthBuilder;
        }
    }
}