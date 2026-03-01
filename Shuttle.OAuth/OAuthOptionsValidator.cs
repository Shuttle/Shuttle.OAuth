using Microsoft.Extensions.Options;

namespace Shuttle.OAuth;

public class OAuthOptionsValidator : IValidateOptions<OAuthOptions>
{
    public ValidateOptionsResult Validate(string? name, OAuthOptions oauthOptions)
    {
        foreach (var provider in oauthOptions.Providers)
        {
            var providerOptions = provider.Value;

            if (string.IsNullOrWhiteSpace(providerOptions.DisplayName))
            {
                providerOptions.DisplayName = provider.Key;
            }

            if (string.IsNullOrWhiteSpace(providerOptions.RedirectUri))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{provider.Key}.RedirectUrl"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Scope))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{provider.Key}.Scope"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Authorize.Url))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{provider.Key}.Authorize.Url"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.ClientId))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{provider.Key}.ClientId"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Token.Url))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{provider.Key}.Token.Url"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Token.ContentTypeHeader))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{provider.Key}.Token.ContentTypeHeader"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Data.Url))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{provider.Key}.Data.Url"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Data.AcceptHeader))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{provider.Key}.Data.AcceptHeader"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Data.AuthorizationHeaderScheme))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{provider.Key}.Data.AuthorizationHeaderScheme"));
            }
        }

        return ValidateOptionsResult.Success;
    }
}