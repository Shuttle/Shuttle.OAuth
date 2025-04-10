using System;
using System.Collections.Generic;

namespace Shuttle.OAuth;

public class OAuthOptions
{
    public const string SectionName = "Shuttle:OAuth";

    public string DefaultRedirectUri { get; set; } = string.Empty;
    public List<OAuthProviderOptions> Providers { get; set; } = [];
}

public class OAuthProviderOptions
{
    public AuthorizeOptions Authorize { get; set; } = new();
    public DataOptions Data { get; set; } = new();
    public string Name { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public TokenOptions Token { get; set; } = new();
}

public class DataOptions
{
    public string AcceptHeader { get; set; } = "application/json";
    public string AuthorizationHeaderScheme { get; set; } = "token";
    public string EMailPropertyName { get; set; } = "email";
    public string IdentityPropertyName { get; set; } = "userPrincipalName";
    public string Url { get; set; } = string.Empty;
}

public class TokenOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string ContentTypeHeader { get; set; } = "application/json";
    public string OriginHeader { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public class AuthorizeOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string CodeChallengeMethod { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}