using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RestSharp;
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
    public async Task<OAuthGrant> RegisterAsync(string providerName, IDictionary<string, string>? data = null)
    {
        var oauthProviderOptions = _oauthOptions.GetProviderOptions(providerName);

        var grant = new OAuthGrant(Guid.NewGuid(), providerName, data);

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

        var oauthProviderOptions = _oauthOptions.GetProviderOptions(grant.ProviderName);

        using var httpClient = _httpClientFactory.CreateClient("Shuttle.OAuth");
        using var client = new RestClient(httpClient);

        var tokenRequest = new RestRequest(oauthProviderOptions.Token.Url)
        {
            Method = Method.Post,
            RequestFormat = DataFormat.Json
        };

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

                tokenRequest.AddHeader("origin", originHeader);
                tokenRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
                tokenRequest.AddParameter("application/x-www-form-urlencoded", parameterBody.ToString(), ParameterType.RequestBody);
                break;
            }
            default:
            {
                tokenRequest.AddHeader("content-type", "application/json");

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

                tokenRequest.AddJsonBody(body);

                break;
            }
        }

        var tokenResponse = await client.ExecuteAsync(tokenRequest);

        // We need to parse the response as dynamic/JsonElement to get the access token
        // Since we are inside a generic method, we can't easily rely on RestSharp's AsDynamic() for intermediate steps safely if we want to be clean, 
        // but for now let's stick to standard behavior.

        dynamic? accessToken;

        try
        {
            // Using System.Text.Json to parse and extract access_token
            using var doc = JsonDocument.Parse(tokenResponse.Content ?? "{}");
            if (doc.RootElement.TryGetProperty("access_token", out var accessTokenElement))
            {
                accessToken = accessTokenElement.GetString();
            }
            else
            {
                throw new InvalidOperationException(string.Format(Resources.AccessTokenNotFoundException, tokenResponse.Content));
            }
        }
        catch
        {
            throw new InvalidOperationException(string.Format(Resources.AccessTokenNotFoundException, tokenResponse.Content));
        }

        var userRequest = new RestRequest(oauthProviderOptions.Data.Url);

        if (!string.IsNullOrWhiteSpace(oauthProviderOptions.Data.AcceptHeader))
        {
            userRequest.AddHeader("Accept", oauthProviderOptions.Data.AcceptHeader);
        }

        userRequest.AddHeader("Authorization", $"{(!string.IsNullOrWhiteSpace(oauthProviderOptions.Data.AuthorizationHeaderScheme) ? $"{oauthProviderOptions.Data.AuthorizationHeaderScheme} " : string.Empty)}{accessToken}");

        // If T is dynamic, we can use RestSharp's ExecuteAsync generic or just return deserialized
        if (typeof(T) == typeof(object) || typeof(T).FullName == "System.Object") // dynamic is basically object
        {
            var response = await client.ExecuteAsync(userRequest);
            return JsonSerializer.Deserialize<T>(response.Content ?? "{}")!;
        }

        return (await client.ExecuteAsync<T>(userRequest)).Data!;
    }
}