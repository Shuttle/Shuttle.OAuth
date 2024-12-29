using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using RestSharp;
using Shuttle.Core.Contract;
using System.Threading.Tasks;

namespace Shuttle.OAuth
{
    public class OAuthService : IOAuthService
    {
        private readonly IEnumerable<ICodeChallenge> _codeChallenges;
        private readonly RestClient _client = new();
        private readonly IOptionsMonitor<OAuthOptions> _oauthOptions;
        private readonly IOAuthGrantRepository _oauthGrantRepository;

        public OAuthService(IOptionsMonitor<OAuthOptions> oauthOptions, IOAuthGrantRepository oauthGrantRepository, IEnumerable<ICodeChallenge> codeChallenges)
        {
            _codeChallenges = Guard.AgainstNull(codeChallenges);
            _oauthOptions = Guard.AgainstNull(oauthOptions);
            _oauthGrantRepository = Guard.AgainstNull(oauthGrantRepository);
        }

        public async Task<OAuthGrant> RegisterAsync(string providerName)
        {
            var oauthOptions = _oauthOptions.Get(providerName);

            var grant = new OAuthGrant(Guid.NewGuid(), providerName);

            if (!string.IsNullOrWhiteSpace(oauthOptions.CodeChallengeMethod))
            {
                var codeChallenge = _codeChallenges.FirstOrDefault(challenge => challenge.Method == oauthOptions.CodeChallengeMethod);

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

            var oauthOptions = _oauthOptions.Get(grant.ProviderName);

            var tokenRequest = new RestRequest(oauthOptions.TokenUrl)
            {
                Method = Method.Post,
                RequestFormat = DataFormat.Json
            };

            switch (oauthOptions.TokenContentType.ToUpperInvariant())
            {
                case "APPLICATION/X-WWW-FORM-URLENCODED":
                {
                    tokenRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
                    tokenRequest.AddParameter("application/x-www-form-urlencoded",
                        $"client_id={oauthOptions.ClientId}&client_secret={oauthOptions.ClientSecret}&grant_type=authorization_code&code={code}{(string.IsNullOrWhiteSpace(grant.CodeVerifier) ? string.Empty : $"&code_verifier={grant.CodeVerifier}")}",
                        ParameterType.RequestBody);
                    break;
                }
                default:
                {
                    tokenRequest.AddHeader("content-type", "application/json");
                    tokenRequest.AddJsonBody(new
                    {
                        client_id = oauthOptions.ClientId,
                        client_secret = oauthOptions.ClientSecret,
                        grant_type = "authorization_code",
                        code
                    });
                    break;
                }
            }

            var tokenResponse = (await _client.ExecuteAsync(tokenRequest)).AsDynamic();

            var userRequest = new RestRequest(oauthOptions.DataUrl);

            if (!string.IsNullOrWhiteSpace(oauthOptions.DataAccept))
            {
                userRequest.AddHeader("Accept", oauthOptions.DataAccept);
            }

            if (tokenResponse.GetType().GetProperty("access_token") == null)
            {
                throw new InvalidOperationException(string.Format(Resources.AccessTokenNotFoundException));
            }

            userRequest.AddHeader("Authorization", $"{(!string.IsNullOrWhiteSpace(oauthOptions.DataAuthorizationScheme) ? $"{oauthOptions.DataAuthorizationScheme} " : string.Empty)}{tokenResponse.GetProperty("access_token")}");

            return (await _client.ExecuteAsync(userRequest)).AsDynamic();
        }
    }
}