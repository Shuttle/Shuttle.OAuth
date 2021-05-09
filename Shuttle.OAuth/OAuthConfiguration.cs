using Shuttle.Core.Contract;

namespace Shuttle.OAuth
{
    public class OAuthConfiguration : IOAuthConfiguration
    {
        public string Name { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TokenUrl { get; set; }
        public string DataUrl { get; set; }
    }
}