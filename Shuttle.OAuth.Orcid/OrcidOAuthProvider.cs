using System;
using RestSharp;
using Shuttle.Core.Contract;

namespace Shuttle.OAuth.Orcid
{
    public class OrcidOAuthProvider:IOAuthProvider
    {
        private readonly IOAuthConfigurationProvider _oauthConfigurationProvider;
        private readonly RestClient _client = new RestClient();

        public OrcidOAuthProvider(IOAuthConfigurationProvider oauthConfigurationProvider)
        {
            Guard.AgainstNull(oauthConfigurationProvider, nameof(oauthConfigurationProvider));

            _oauthConfigurationProvider = oauthConfigurationProvider;
        }

        public dynamic GetData(string code)
        {
            Guard.AgainstNullOrEmptyString(code, nameof(code));

            var credentials = _oauthConfigurationProvider.Get("orcid");

            var tokenRequest = new RestRequest(credentials.GetTokenUrl($"oauth/token"))
            {
                Method = Method.POST,
                RequestFormat = DataFormat.Json,
            };

            tokenRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
            tokenRequest.AddParameter("application/x-www-form-urlencoded", $"client_id={credentials.ClientId}&client_secret={credentials.ClientSecret}&grant_type=authorization_code&code={code}", ParameterType.RequestBody);

            var tokenResponse = _client.Execute(tokenRequest).AsDynamic();

            if (tokenResponse == null || tokenResponse.orcid == null)
            {
                return null;
            }

            var userRequest = new RestRequest(credentials.GetDataUrl($"{tokenResponse.orcid.ToString()}/email"));

            userRequest.AddHeader("Accept", "application/vnd.orcid+json");
            userRequest.AddHeader("Authorization", $"Bearer {tokenResponse.access_token}");

            return _client.Execute(userRequest).AsDynamic();
        }

        public string Name => "Orcid";
    }
}
