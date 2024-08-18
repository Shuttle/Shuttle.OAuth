using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RestSharp;
using Shuttle.Core.Contract;

namespace Shuttle.OAuth.GitHub
{
    public class GitHubOAuthProvider : IOAuthProvider
    {
        private readonly RestClient _client = new RestClient();
        private readonly OAuthOptions _oauthOptions;

        public GitHubOAuthProvider(IOptionsMonitor<OAuthOptions> oauthOptions)
        {
            Guard.AgainstNull(oauthOptions);

            _oauthOptions = oauthOptions.Get(Name);
        }

        public async Task<dynamic?> GetDataDynamicAsync(string code)
        {
            Guard.AgainstNullOrEmptyString(code, nameof(code));

            var tokenRequest = new RestRequest(_oauthOptions.TokenUrl)
            {
                Method = Method.Post,
                RequestFormat = DataFormat.Json,
            };

            tokenRequest.AddHeader("content-type", "application/json");
            tokenRequest.AddJsonBody(new
            {
                client_id = _oauthOptions.ClientId,
                client_secret = _oauthOptions.ClientSecret,
                code
            });

            var tokenResponse = (await _client.ExecuteAsync(tokenRequest)).AsDynamic();

            if (tokenResponse == null)
            {
                return null;
            }

            var userRequest = new RestRequest(_oauthOptions.DataUrl);

            userRequest.AddHeader("Authorization", $"token {tokenResponse.access_token}");

            return (await _client.ExecuteAsync(userRequest)).AsDynamic();
        }

        public string Name => "GitHub";
    }
}
