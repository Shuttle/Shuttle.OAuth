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

        foreach (var providerOptions in oauthOptions.Providers)
        {
            if (string.IsNullOrWhiteSpace(providerOptions.Name))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, Resources.OAuthProivderOptionsNameException));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.RedirectUri))
            {
                providerOptions.RedirectUri = oauthOptions.DefaultRedirectUri;
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Scope))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{providerOptions.Name}.Scope"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Authorize.Url))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{providerOptions.Name}.Authorize.Url"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Authorize.ClientId))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{providerOptions.Name}.Authorize.ClientId"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Token.Url))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{providerOptions.Name}.Token.Url"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Token.ContentTypeHeader))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{providerOptions.Name}.Token.ContentTypeHeader"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Data.Url))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{providerOptions.Name}.Data.Url"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Data.AcceptHeader))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{providerOptions.Name}.Data.AcceptHeader"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Data.AuthorizationHeaderScheme))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{providerOptions.Name}.Data.AuthorizationHeaderScheme"));
            }

            if (string.IsNullOrWhiteSpace(providerOptions.Data.EMailPropertyName))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{providerOptions.Name}.Data.EMailPropertyName"));
            }
        }

        return ValidateOptionsResult.Success;
    }
}