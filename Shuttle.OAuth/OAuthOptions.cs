namespace Shuttle.OAuth;

/// <summary>
///     Configuration options for the OAuth service.
/// </summary>
public class OAuthOptions
{
    public const string SectionName = "Shuttle:OAuth";

    /// <summary>
    ///     The default redirect URI to use if none is specified in the grant or provider options.
    /// </summary>
    public string DefaultRedirectUri { get; set; } = string.Empty;

    /// <summary>
    ///     A list of configured OAuth providers.
    /// </summary>
    public Dictionary<string, OAuthProviderOptions> Providers { get; set; } = new();
}

/// <summary>
///     Configuration options for a specific OAuth provider.
/// </summary>
public class OAuthProviderOptions
{
    /// <summary>
    ///     Configuration for the authorization endpoint.
    /// </summary>
    public AuthorizeOptions Authorize { get; set; } = new();

    /// <summary>
    ///     The client ID issued by the provider.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    ///     The client secret issued by the provider.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    ///     Configuration for retrieving user data.
    /// </summary>
    public DataOptions Data { get; set; } = new();

    /// <summary>
    ///     The groups that the options belong to.
    /// </summary>
    public List<string> Groups { get; set; } = [];

    /// <summary>
    ///     The display name of the provider (e.g., "Google", "Microsoft").
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    ///     The redirect URI for this provider.
    /// </summary>
    public string RedirectUri { get; set; } = string.Empty;

    /// <summary>
    ///     The scope(s) to request during authorization.
    /// </summary>
    public string Scope { get; set; } = string.Empty;

    /// <summary>
    ///     Configuration for the token endpoint.
    /// </summary>
    public TokenOptions Token { get; set; } = new();

    public string SvgFileName { get; set; } = string.Empty;
}

/// <summary>
///     Configuration for the user data endpoint.
/// </summary>
public class DataOptions
{
    /// <summary>
    ///     The value for the Accept header. Defaults to "application/json".
    /// </summary>
    public string AcceptHeader { get; set; } = "application/json";

    /// <summary>
    ///     The scheme to use for the Authorization header. Defaults to "token".
    /// </summary>
    public string AuthorizationHeaderScheme { get; set; } = "token";

    /// <summary>
    ///     The name of the property containing the email address. Defaults to "email".
    /// </summary>
    public string EmailPropertyName { get; set; } = "email";

    /// <summary>
    ///     The name of the property containing the user identity. Defaults to "userPrincipalName".
    /// </summary>
    public string IdentityPropertyName { get; set; } = "userPrincipalName";

    /// <summary>
    ///     The URL of the user data endpoint.
    /// </summary>
    public string Url { get; set; } = string.Empty;
}

/// <summary>
///     Configuration for the token endpoint.
/// </summary>
public class TokenOptions
{
    /// <summary>
    ///     The Content-Type header to use when requesting the token. Defaults to "application/json".
    /// </summary>
    public string ContentTypeHeader { get; set; } = "application/json";

    /// <summary>
    ///     The origin header to send if required.
    /// </summary>
    public string OriginHeader { get; set; } = string.Empty;

    /// <summary>
    ///     The URL of the token endpoint.
    /// </summary>
    public string Url { get; set; } = string.Empty;
}

/// <summary>
///     Configuration for the authorization endpoint.
/// </summary>
public class AuthorizeOptions
{
    /// <summary>
    ///     The code challenge method (e.g., "S256").
    /// </summary>
    public string CodeChallengeMethod { get; set; } = string.Empty;

    /// <summary>
    ///     The URL of the authorization endpoint.
    /// </summary>
    public string Url { get; set; } = string.Empty;
}