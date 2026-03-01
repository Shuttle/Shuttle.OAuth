using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.OAuth;

/// <summary>
///     Provides functionality for registering OAuth grants and retrieving user data.
/// </summary>
public class OAuthService(IOptions<OAuthOptions> oauthOptions, IOAuthGrantRepository oauthGrantRepository, IEnumerable<ICodeChallenge> codeChallenges, IHttpClientFactory httpClientFactory)
    : IOAuthService
{
    private readonly IEnumerable<ICodeChallenge> _codeChallenges = Guard.AgainstNull(codeChallenges);
    private readonly IHttpClientFactory _httpClientFactory = Guard.AgainstNull(httpClientFactory);
    private readonly IOAuthGrantRepository _oauthGrantRepository = Guard.AgainstNull(oauthGrantRepository);
    private readonly OAuthOptions _oauthOptions = Guard.AgainstNull(Guard.AgainstNull(oauthOptions).Value);

    /// <inheritdoc />
    public async Task<OAuthGrant> RegisterAsync(string groupName, string providerName, IDictionary<string, string>? data = null)
    {
        var oauthProviderOptions = _oauthOptions.GetProviderOptions(groupName, providerName);

        var grant = new OAuthGrant(Guid.NewGuid(), groupName, providerName, data);

        if (!string.IsNullOrWhiteSpace(oauthProviderOptions.Authorize.CodeChallengeMethod))
        {
            var codeChallenge = _codeChallenges.FirstOrDefault(challenge => challenge.Method == oauthProviderOptions.Authorize.CodeChallengeMethod);

            if (codeChallenge == null)
            {
                throw new InvalidOperationException(string.Format(Resources.CodeChallengeNotFoundException, codeChallenge));
            }

            var codeVerifier = await codeChallenge.GenerateCodeVerifierAsync();

            grant.WithCodeChallenge(await codeChallenge.GenerateCodeChallengeAsync(codeVerifier), codeVerifier);
        }

        await _oauthGrantRepository.SaveAsync(grant);

        return grant;
    }

    /// <inheritdoc />
    public async Task<dynamic> GetDataAsync(OAuthGrant grant, string code)
    {
        return await GetDataAsync<dynamic>(grant, code);
    }

    /// <inheritdoc />
    public async Task<T> GetDataAsync<T>(OAuthGrant grant, string code)
    {
        Guard.AgainstNull(grant);
        Guard.AgainstEmpty(code);

        var oauthProviderOptions = _oauthOptions.GetProviderOptions(grant.GroupName, grant.ProviderName);

        using var httpClient = _httpClientFactory.CreateClient("Shuttle.OAuth");
        
        var tokenRequest = new HttpRequestMessage(HttpMethod.Post, oauthProviderOptions.Token.Url);

        switch (oauthProviderOptions.Token.ContentTypeHeader.ToUpperInvariant())
        {
            case "APPLICATION/X-WWW-FORM-URLENCODED":
            {
                var redirectUri = grant.HasData("RedirectUri") ? grant.GetData("RedirectUri") : oauthProviderOptions.RedirectUri;
                var parameterBody = new StringBuilder($"client_id={oauthProviderOptions.ClientId}&grant_type=authorization_code&code={code}&redirect_uri={redirectUri}");

                if (!string.IsNullOrWhiteSpace(grant.CodeVerifier))
                {
                    parameterBody.Append($"&code_verifier={grant.CodeVerifier}");
                }

                var originHeader = oauthProviderOptions.Token.OriginHeader;

                if (string.IsNullOrWhiteSpace(originHeader))
                {
                    if (!Uri.TryCreate(redirectUri, UriKind.Absolute, out var uri))
                    {
                        throw new ArgumentException(@"Invalid URI", nameof(redirectUri));
                    }

                    originHeader = $"{uri.Scheme}://{uri.Host}";

                    if (!uri.IsDefaultPort)
                    {
                        originHeader += $":{uri.Port}";
                    }
                }

                tokenRequest.Headers.Add("Origin", originHeader);
                tokenRequest.Content = new StringContent(parameterBody.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");
                
                break;
            }
            default:
            {
                var body = new Dictionary<string, object>
                {
                    { "client_id", oauthProviderOptions.ClientId },
                    { "grant_type", "authorization_code" },
                    { "code", code }
                };

                if (!string.IsNullOrWhiteSpace(oauthProviderOptions.ClientSecret))
                {
                    body["client_secret"] = oauthProviderOptions.ClientSecret;
                }

                tokenRequest.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

                break;
            }
        }

        var tokenResponse = await httpClient.SendAsync(tokenRequest);
        var content = await tokenResponse.Content.ReadAsStringAsync();

        if (!tokenResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(string.Format(Resources.AccessTokenNotFoundException, content));
        }

        dynamic? accessToken;

        try
        {
            using var doc = JsonDocument.Parse(content);
            
            if (doc.RootElement.TryGetProperty("access_token", out var accessTokenElement))
            {
                accessToken = accessTokenElement.GetString();
            }
            else
            {
                throw new InvalidOperationException(string.Format(Resources.AccessTokenNotFoundException, content));
            }
        }
        catch
        {
            throw new InvalidOperationException(string.Format(Resources.AccessTokenNotFoundException, content));
        }

        var userRequest = new HttpRequestMessage(HttpMethod.Get, oauthProviderOptions.Data.Url);

        if (!string.IsNullOrWhiteSpace(oauthProviderOptions.Data.AcceptHeader))
        {
            userRequest.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(oauthProviderOptions.Data.AcceptHeader));
        }

        userRequest.Headers.Authorization = new AuthenticationHeaderValue(!string.IsNullOrWhiteSpace(oauthProviderOptions.Data.AuthorizationHeaderScheme) ? oauthProviderOptions.Data.AuthorizationHeaderScheme : "Bearer", accessToken);

        var response = await httpClient.SendAsync(userRequest);
        
        content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(string.Format(Resources.GetDataException, content));
        }

        return JsonSerializer.Deserialize<T>(content)!;
    }
}