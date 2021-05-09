using System;
using RestSharp;
using Shuttle.Core.Contract;

namespace Shuttle.OAuth.GitHub
{
    public class GitHubOAuthProvider : IOAuthProvider
    {
        private readonly IOAuthConfigurationProvider _oauthConfigurationProvider;
        private readonly RestClient _client = new RestClient();

        public GitHubOAuthProvider(IOAuthConfigurationProvider oauthConfigurationProvider)
        {
            Guard.AgainstNull(oauthConfigurationProvider, nameof(oauthConfigurationProvider));
            
            _oauthConfigurationProvider = oauthConfigurationProvider;
        }

        public dynamic GetData(string code)
        {
            Guard.AgainstNullOrEmptyString(code, nameof(code));

            var configuration = _oauthConfigurationProvider.Get("github");

            var tokenRequest = new RestRequest(configuration.TokenUrl)
            {
                Method = Method.POST,
                RequestFormat = DataFormat.Json,
            };

            tokenRequest.AddHeader("content-type", "application/json");
            tokenRequest.AddJsonBody(new
            {
                client_id = configuration.ClientId,
                client_secret = configuration.ClientSecret,
                code
            });

            var tokenResponse = _client.Execute(tokenRequest).AsDynamic();

            var userRequest = new RestRequest(configuration.DataUrl);

            userRequest.AddHeader("Authorization", $"token {tokenResponse.access_token}");

            return _client.Execute(userRequest).AsDynamic();
        }

        public string Name => "GitHub";
    }
}
