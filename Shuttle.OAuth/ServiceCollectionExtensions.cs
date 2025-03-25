using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.OAuth;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryOAuthGrantRepository(this IServiceCollection services)
    {
        Guard.AgainstNull(services);

        services.AddSingleton<IOAuthGrantRepository, InMemoryOAuthGrantRepository>();

        return services;
    }

    public static IServiceCollection AddOAuth(this IServiceCollection services, Action<OAuthBuilder>? builder = null)
    {
        Guard.AgainstNull(services);

        var accessBuilder = new OAuthBuilder(services);

        builder?.Invoke(accessBuilder);

        services.Configure<OAuthOptions>(options =>
        {
            options.DefaultRedirectUri = accessBuilder.Options.DefaultRedirectUri;
            options.Providers = accessBuilder.Options.Providers;
        });

        services.AddSingleton<IValidateOptions<OAuthOptions>, OAuthOptionsValidator>();
        services.AddSingleton<IOAuthService, OAuthService>();
        services.AddSingleton<ICodeChallenge, S256CodeChallenge>();

        services.AddHttpClient("Shuttle.OAuth");

        return services;
    }
}