namespace Shuttle.OAuth.GitHub
{
    public class GitHubData
    {
        public static string EMailAddress(dynamic data)
        {
            return data.email;
        }
    }
}