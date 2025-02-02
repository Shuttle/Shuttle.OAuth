using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RestSharp;
using Shuttle.Core.Contract;

namespace Shuttle.OAuth;

public class OAuthService : IOAuthService
{
    private readonly RestClient _client = new();
    private readonly IEnumerable<ICodeChallenge> _codeChallenges;
    private readonly IOAuthGrantRepository _oauthGrantRepository;
    private readonly OAuthOptions _oauthOptions;

    public OAuthService(IOptions<OAuthOptions> oauthOptions, IOAuthGrantRepository oauthGrantRepository, IEnumerable<ICodeChallenge> codeChallenges)
    {
        _codeChallenges = Guard.AgainstNull(codeChallenges);
        _oauthOptions = Guard.AgainstNull(Guard.AgainstNull(oauthOptions).Value);
        _oauthGrantRepository = Guard.AgainstNull(oauthGrantRepository);
    }

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

    public async Task<dynamic> GetDataAsync(OAuthGrant grant, string code)
    {
        Guard.AgainstNull(grant);
        Guard.AgainstNullOrEmptyString(code);

        var oauthProviderOptions = _oauthOptions.GetProviderOptions(grant.ProviderName);

        var tokenRequest = new RestRequest(oauthProviderOptions.Token.Url)
        {
            Method = Method.Post,
            RequestFormat = DataFormat.Json
        };

        switch (oauthProviderOptions.Token.ContentTypeHeader.ToUpperInvariant())
        {
            case "APPLICATION/X-WWW-FORM-URLENCODED":
            {
                var parameterBody = new StringBuilder($"client_id={oauthProviderOptions.Token.ClientId}&grant_type=authorization_code&code={code}&redirect_uri={oauthProviderOptions.RedirectUri}");

                if (!string.IsNullOrWhiteSpace(grant.CodeVerifier))
                {
                    parameterBody.Append($"&code_verifier={grant.CodeVerifier}");
                }

                if (!string.IsNullOrWhiteSpace(oauthProviderOptions.Token.OriginHeader))
                {
                    tokenRequest.AddHeader("origin", oauthProviderOptions.Token.OriginHeader);
                }
                
                tokenRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
                tokenRequest.AddParameter("application/x-www-form-urlencoded", parameterBody.ToString(), ParameterType.RequestBody);
                break;
            }
            default:
            {
                tokenRequest.AddHeader("content-type", "application/json");

                var body = new Dictionary<string, object>
                {
                    { "client_id", oauthProviderOptions.Token.ClientId },
                    { "grant_type", "authorization_code" },
                    { "code", code }
                };

                if (!string.IsNullOrWhiteSpace(oauthProviderOptions.Token.ClientSecret))
                {
                    body["client_secret"] = oauthProviderOptions.Token.ClientSecret;
                }

                tokenRequest.AddJsonBody(body);

                break;
            }
        }

        var tokenResponse = (await _client.ExecuteAsync(tokenRequest)).AsDynamic();

        var userRequest = new RestRequest(oauthProviderOptions.Data.Url);

        if (!string.IsNullOrWhiteSpace(oauthProviderOptions.Data.AcceptHeader))
        {
            userRequest.AddHeader("Accept", oauthProviderOptions.Data.AcceptHeader);
        }

        dynamic? accessToken;

        try
        {
            accessToken = tokenResponse.GetProperty("access_token");
        }
        catch
        {
            throw new InvalidOperationException(string.Format(Resources.AccessTokenNotFoundException, tokenResponse));
        }
        
        userRequest.AddHeader("Authorization", $"{(!string.IsNullOrWhiteSpace(oauthProviderOptions.Data.AuthorizationHeaderScheme) ? $"{oauthProviderOptions.Data.AuthorizationHeaderScheme} " : string.Empty)}{accessToken}");

        return (await _client.ExecuteAsync(userRequest)).AsDynamic();
    }
}