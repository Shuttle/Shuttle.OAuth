using Microsoft.Extensions.Options;

namespace Shuttle.OAuth;

public class OAuthOptionsValidator : IValidateOptions<OAuthOptions>
{
    public ValidateOptionsResult Validate(string? name, OAuthOptions oauthOptions)
    {
        if (string.IsNullOrWhiteSpace(oauthOptions.DefaultRedirectUri))
        {
            return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, "DefaultRedirectUri"));
        }

        foreach (var pair in oauthOptions.Providers)
        {
            var providerOptions = pair.Value;

            if (string.IsNullOrWhiteSpace(providerOptions.DisplayName))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{pair.Key}.DisplayName"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.RedirectUri))
            {
                providerOptions.RedirectUri = oauthOptions.DefaultRedirectUri;
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Scope))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{pair.Key}.Scope"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Authorize.Url))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{pair.Key}.Authorize.Url"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.ClientId))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{pair.Key}.ClientId"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Token.Url))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{pair.Key}.Token.Url"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Token.ContentTypeHeader))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{pair.Key}.Token.ContentTypeHeader"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Data.Url))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{pair.Key}.Data.Url"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Data.AcceptHeader))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{pair.Key}.Data.AcceptHeader"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Data.AuthorizationHeaderScheme))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{pair.Key}.Data.AuthorizationHeaderScheme"));
            }
        }

        return ValidateOptionsResult.Success;
    }
}