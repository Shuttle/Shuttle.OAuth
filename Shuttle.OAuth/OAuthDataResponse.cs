using Shuttle.Core.Contract;

namespace Shuttle.OAuth
{
    internal class OAuthDataResponse
    {
        private OAuthDataResponse()
        {
        }

        public string Message { get; private set; }

        public bool Ok => string.IsNullOrWhiteSpace(Message) && !string.IsNullOrWhiteSpace(Value);

        public string Value { get; private set; }

        public bool HasMessage => !string.IsNullOrWhiteSpace(Message);

        public static OAuthDataResponse Success(string value)
        {
            Guard.AgainstNullOrEmptyString(value, nameof(value));

            return new OAuthDataResponse
            {
                Value = value
            };
        }

        public static OAuthDataResponse Failure()
        {
            return new OAuthDataResponse();
        }

        public static OAuthDataResponse Failure(string message)
        {
            Guard.AgainstNullOrEmptyString(message, nameof(message));

            return new OAuthDataResponse
            {
                Message = message
            };
        }
    }
}