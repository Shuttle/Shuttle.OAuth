using Microsoft.Extensions.DependencyInjection;
using Shuttle.Contract;

namespace Shuttle.OAuth;

public class OAuthBuilder(IServiceCollection services)
{
    public OAuthOptions Options
    {
        get;
        set => field = value ?? throw new ArgumentNullException(nameof(value));
    } = new();

    public IServiceCollection Services { get; } = Guard.AgainstNull(services);
}