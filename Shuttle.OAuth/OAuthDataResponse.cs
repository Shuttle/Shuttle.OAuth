using Shuttle.Core.Contract;

namespace Shuttle.OAuth;

/// <summary>
///     Represents a response from an OAuth data request.
/// </summary>
internal class OAuthDataResponse
{
    private OAuthDataResponse()
    {
    }

    /// <summary>
    ///     Indicates whether the response contains an error message.
    /// </summary>
    public bool HasMessage => !string.IsNullOrWhiteSpace(Message);

    /// <summary>
    ///     The error message, if any.
    /// </summary>
    public string Message { get; private init; } = string.Empty;

    /// <summary>
    ///     Indicates whether the response is successful (no error message and a value is present).
    /// </summary>
    public bool Ok => string.IsNullOrWhiteSpace(Message) && !string.IsNullOrWhiteSpace(Value);

    /// <summary>
    ///     The response value.
    /// </summary>
    public string Value { get; private init; } = string.Empty;

    /// <summary>
    ///     Creates a failure response with no message.
    /// </summary>
    /// <returns>A failure OAuthDataResponse.</returns>
    public static OAuthDataResponse Failure()
    {
        return new();
    }

    /// <summary>
    ///     Creates a failure response with the specified message.
    /// </summary>
    /// <param name="message">The failure message.</param>
    /// <returns>A failure OAuthDataResponse.</returns>
    public static OAuthDataResponse Failure(string message)
    {
        Guard.AgainstEmpty(message);

        return new()
        {
            Message = message
        };
    }

    /// <summary>
    ///     Creates a success response with the specified value.
    /// </summary>
    /// <param name="value">The success value.</param>
    /// <returns>A success OAuthDataResponse.</returns>
    public static OAuthDataResponse Success(string value)
    {
        Guard.AgainstEmpty(value);

        return new()
        {
            Value = value
        };
    }
}