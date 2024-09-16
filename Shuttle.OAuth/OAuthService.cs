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
        private readonly RestClient _client = new RestClient();
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

        public async Task<dynamic?> GetDataDynamicAsync(Guid requestId, string code)
        {
            Guard.AgainstNullOrEmptyString(code);

            var grant = await _oauthGrantRepository.GetAsync(requestId);

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
                        $"client_id={oauthOptions.ClientId}&client_secret={oauthOptions.ClientSecret}&grant_type=authorization_code&code={code}",
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
                        code
                    });
                    break;
                }
            }

            var tokenResponse = (await _client.ExecuteAsync(tokenRequest)).AsDynamic();

            if (tokenResponse == null)
            {
                return null;
            }

            var userRequest = new RestRequest(oauthOptions.DataUrl);

            if (!string.IsNullOrWhiteSpace(oauthOptions.DataAccept))
            {
                userRequest.AddHeader("Accept", oauthOptions.DataAccept);
            }

            userRequest.AddHeader("Authorization", $"{(!string.IsNullOrWhiteSpace(oauthOptions.DataAuthorization) ? $"{oauthOptions.DataAuthorization} " : string.Empty)}{tokenResponse.access_token}");

            return (await _client.ExecuteAsync(userRequest)).AsDynamic();
        }
    }
}