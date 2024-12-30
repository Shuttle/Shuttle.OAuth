using System.Collections.Generic;

namespace Shuttle.OAuth;

public class OAuthOptions
{
    public const string SectionName = "Shuttle:OAuth";

    public string DefaultRedirectUri { get; set; } = default!;
    public List<OAuthProviderOptions> Providers { get; set; } = [];
}

public class OAuthProviderOptions
{
    public string Name { get; set; } = default!;
    public AuthorizeOptions Authorize { get; set; } = new();
    public TokenOptions Token { get; set; } = new();
    public DataOptions Data { get; set; } = new();
    public string RedirectUri { get; set; } = default!;
    public string Scope { get; set; } = default!;
}

public class DataOptions
{
    public string Url { get; set; } = default!;
    public string AcceptHeader { get; set; } = "application/json";
    public string AuthorizationHeaderScheme { get; set; } = "token";
    public string EMailPropertyName { get; set; } = "email";
}

public class TokenOptions
{
    public string Url { get; set; } = default!;
    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
    public string ContentTypeHeader { get; set; } = "application/json";
    public string OriginHeader { get; set; } = default!;
}

public class AuthorizeOptions
{
    public string Url { get; set; } = default!;
    public string ClientId { get; set; } = default!;
    public string CodeChallengeMethod { get; set; } = default!;
}