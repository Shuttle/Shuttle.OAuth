using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;
using System;

namespace Shuttle.OAuth
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOAuth(this IServiceCollection services, Action<OAuthBuilder>? builder = null)
        {
            Guard.AgainstNull(services);

            var accessBuilder = new OAuthBuilder(services);

            builder?.Invoke(accessBuilder);

            services.AddSingleton<IOAuthProviderService, OAuthProviderService>();

            return services;
        }
    }
}