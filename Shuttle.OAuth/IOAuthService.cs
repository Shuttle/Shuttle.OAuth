namespace Shuttle.OAuth;

/// <summary>
///     Defines the contract for an OAuth service.
/// </summary>
public interface IOAuthService
{
    /// <summary>
    ///     Exchanges an authorization code for an access token and retrieves the user data as a dynamic object.
    /// </summary>
    /// <param name="grant">The OAuth grant associated with the request.</param>
    /// <param name="code">The authorization code received from the provider.</param>
    /// <returns>The user data as a dynamic object.</returns>
    Task<dynamic> GetDataAsync(OAuthGrant grant, string code);

    /// <summary>
    ///     Exchanges an authorization code for an access token and retrieves the user data as a strongly-typed object.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the user data into.</typeparam>
    /// <param name="grant">The OAuth grant associated with the request.</param>
    /// <param name="code">The authorization code received from the provider.</param>
    /// <returns>The user data deserialized into type T.</returns>
    Task<T> GetDataAsync<T>(OAuthGrant grant, string code);

    /// <summary>
    ///     Registers a new OAuth session/grant for the specified provider.
    /// </summary>
    /// <param name="groupName">The name of the provider group.</param>
    /// <param name="providerName">The name of the provider to use.</param>
    /// <param name="data">Optional dictionary of data to persist with the grant.</param>
    /// <returns>The created OAuthGrant.</returns>
    Task<OAuthGrant> RegisterAsync(string groupName, string providerName, IDictionary<string, string>? data = null);
}