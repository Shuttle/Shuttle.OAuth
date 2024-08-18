using Shuttle.Core.Contract;

namespace Shuttle.OAuth
{
    public static class OAuthOptionsExtensions
    {
        public static string GetTokenUrl(this OAuthOptions options, string relativePath)
        {
            Guard.AgainstNull(options);
            Guard.AgainstNull(options.TokenUrl);
            Guard.AgainstNull(relativePath);

            return $"{options.TokenUrl}{(options.TokenUrl.EndsWith("/") ? string.Empty : "/")}{(relativePath.StartsWith("/") ? relativePath : relativePath[1..])}";
        }

        public static string GetDataUrl(this OAuthOptions options, string relativePath)
        {
            Guard.AgainstNull(options);
            Guard.AgainstNull(options.TokenUrl);

            return $"{options.DataUrl}{(options.DataUrl.EndsWith("/") ? string.Empty : "/")}{(relativePath.StartsWith("/") ? relativePath : relativePath[1..])}";
        }
    }
}