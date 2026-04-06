using Microsoft.Extensions.Logging;

namespace Shuttle.OAuth;


public static class LogMessage
{
    private static readonly Action<ILogger, Guid, Exception?> GetGrantDelegate =
        LoggerMessage.Define<Guid>(LogLevel.Trace, new(1000, nameof(GetGrant)), "Retrieving grant with id '{GrantId}'.");

    private static readonly Action<ILogger, Guid, string, string, Exception?> GetDataDelegate =
        LoggerMessage.Define<Guid, string, string>(LogLevel.Trace, new(1001, nameof(GetData)), "Retrieving data for grant with id '{GrantId}' from provider '{ProviderName}' using code '{Code}'.");

    private static readonly Action<ILogger, Guid, string, Exception?> RegisterDelegate =
        LoggerMessage.Define<Guid, string>(LogLevel.Trace, new(1002, nameof(Register)), "Registering grant with id '{GrantId}' for provider '{ProviderName}'.");

    public static void GetGrant(ILogger logger, Guid grantId) =>
        GetGrantDelegate(logger, grantId, null);

    public static void GetData(ILogger logger, Guid grantId, string providerName, string code) =>
        GetDataDelegate(logger, grantId, providerName, code, null);

    public static void Register(ILogger logger, Guid grantId, string providerName) =>
        RegisterDelegate(logger, grantId, providerName, null);
}