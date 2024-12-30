using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;
using System;

namespace Shuttle.OAuth;

public class OAuthBuilder
{
    private OAuthOptions _oauthOptions = new();

    public OAuthBuilder(IServiceCollection services)
    {
        Services = Guard.AgainstNull(services);
    }

    public IServiceCollection Services { get; }

    public OAuthOptions Options
    {
        get => _oauthOptions;
        set => _oauthOptions = value ?? throw new ArgumentNullException(nameof(value));
    }
}