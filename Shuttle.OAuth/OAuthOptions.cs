namespace Shuttle.OAuth
{
    public class OAuthOptions
    {
        public const string SectionName = "Shuttle:OAuth";
        public string ClientId { get; set; } = default!;
        public string ClientSecret { get; set; } = default!;
        public string TokenUrl { get; set; } = default!;
        public string DataUrl { get; set; } = default!;
    }
}