using Newtonsoft.Json;
using RestSharp;
using Shuttle.Core.Contract;

namespace Shuttle.OAuth
{
    public static class RestResponseExtensions
    {
        public static dynamic AsDynamic(this RestResponse response)
        {
            Guard.AgainstNull(response, nameof(response));

            return JsonConvert.DeserializeObject<dynamic>(response.Content);
        }
    }
}