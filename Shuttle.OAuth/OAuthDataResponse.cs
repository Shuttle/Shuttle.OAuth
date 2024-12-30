using Shuttle.Core.Contract;

namespace Shuttle.OAuth;

internal class OAuthDataResponse
{
    private OAuthDataResponse()
    {
    }

    public bool HasMessage => !string.IsNullOrWhiteSpace(Message);

    public string Message { get; private init; } = string.Empty;

    public bool Ok => string.IsNullOrWhiteSpace(Message) && !string.IsNullOrWhiteSpace(Value);

    public string Value { get; private init; } = string.Empty;

    public static OAuthDataResponse Failure()
    {
        return new();
    }

    public static OAuthDataResponse Failure(string message)
    {
        Guard.AgainstNullOrEmptyString(message);

        return new()
        {
            Message = message
        };
    }

    public static OAuthDataResponse Success(string value)
    {
        Guard.AgainstNullOrEmptyString(value);

        return new()
        {
            Value = value
        };
    }
}