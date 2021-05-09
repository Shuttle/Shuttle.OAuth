using System;

namespace Shuttle.OAuth
{
    public class OAuthException : Exception
    {
        public OAuthException(string message) : base(message)
        {
        }
    }
}