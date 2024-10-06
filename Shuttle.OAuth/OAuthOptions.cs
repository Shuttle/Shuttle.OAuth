namespace Shuttle.OAuth
{
    public class OAuthOptions
    {
        public const string SectionName = "Shuttle:OAuth";
        public string AuthorizationUrl { get; set; } = default!;
        public string ClientId { get; set; } = default!;
        public string ClientSecret { get; set; } = default!;
        public string TokenUrl { get; set; } = default!;
        public string TokenContentType { get; set; } = "application/json";
        public string DataUrl { get; set; } = default!;
        public string DataAuthorization { get; set; } = "token";
        public string DataAccept { get; set; } = "application/json";
        public string CodeChallengeMethod { get; set; } = default!;
        public string Scope { get; set; } = default!;
        public string EMailPropertyName { get; set; } = "email";
    }
}