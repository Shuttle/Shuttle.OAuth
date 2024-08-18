using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RestSharp;
using Shuttle.Core.Contract;

namespace Shuttle.OAuth.Orcid
{
    public class OrcidOAuthProvider : IOAuthProvider
    {
        private readonly RestClient _client = new RestClient();
        private readonly OAuthOptions _oauthOptions;

        public OrcidOAuthProvider(IOptionsMonitor<OAuthOptions> oauthOptions)
        {
            Guard.AgainstNull(oauthOptions);

            _oauthOptions = oauthOptions.Get(Name);
        }

        public async Task<dynamic?> GetDataDynamicAsync(string code)
        {
            Guard.AgainstNullOrEmptyString(code, nameof(code));

            var tokenRequest = new RestRequest(_oauthOptions.GetTokenUrl("oauth/token"))
            {
                Method = Method.Post,
                RequestFormat = DataFormat.Json
            };

            tokenRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
            tokenRequest.AddParameter("application/x-www-form-urlencoded",
                $"client_id={_oauthOptions.ClientId}&client_secret={_oauthOptions.ClientSecret}&grant_type=authorization_code&code={code}",
                ParameterType.RequestBody);

            var tokenResponse = (await _client.ExecuteAsync(tokenRequest)).AsDynamic();

            if (tokenResponse == null || tokenResponse.orcid == null)
            {
                return null;
            }

            var userRequest = new RestRequest(_oauthOptions.GetDataUrl($"{tokenResponse.orcid.ToString()}/email"));

            userRequest.AddHeader("Accept", "application/vnd.orcid+json");
            userRequest.AddHeader("Authorization", $"Bearer {tokenResponse.access_token}");

            return (await _client.ExecuteAsync(userRequest)).AsDynamic();
        }

        public string Name => "Orcid";
    }
}