using Microsoft.Extensions.Options;

namespace Shuttle.OAuth;

public class OAuthOptionsValidator : IValidateOptions<OAuthOptions>
{
    public ValidateOptionsResult Validate(string? name, OAuthOptions oauthOptions)
    {
        foreach (var group in oauthOptions)
        {
            foreach (var provider in group.Value.Providers)
            {
                var providerOptions = provider.Value;

                if (string.IsNullOrWhiteSpace(providerOptions.DisplayName))
                {
                    providerOptions.DisplayName = provider.Key;
                }

                if (string.IsNullOrWhiteSpace(providerOptions.RedirectUri))
                {
                    if (string.IsNullOrWhiteSpace(group.Value.DefaultRedirectUri))
                    {
                        return ValidateOptionsResult.Fail($"Both '{group.Key}:{provider.Key}:RedirectUri' and '{group.Key}:DefaultRedirectUri' are empty.");
                    }

                    providerOptions.RedirectUri = group.Value.DefaultRedirectUri;
                }

                if (string.IsNullOrWhiteSpace(providerOptions.Scope))
                {
                    return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{group.Key}.Scope"));
                }

                if (string.IsNullOrWhiteSpace(providerOptions.Authorize.Url))
                {
                    return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{group.Key}.Authorize.Url"));
                }

                if (string.IsNullOrWhiteSpace(providerOptions.ClientId))
                {
                    return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{group.Key}.ClientId"));
                }

                if (string.IsNullOrWhiteSpace(providerOptions.Token.Url))
                {
                    return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{group.Key}.Token.Url"));
                }

                if (string.IsNullOrWhiteSpace(providerOptions.Token.ContentTypeHeader))
                {
                    return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{group.Key}.Token.ContentTypeHeader"));
                }

                if (string.IsNullOrWhiteSpace(providerOptions.Data.Url))
                {
                    return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{group.Key}.Data.Url"));
                }

                if (string.IsNullOrWhiteSpace(providerOptions.Data.AcceptHeader))
                {
                    return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{group.Key}.Data.AcceptHeader"));
                }

                if (string.IsNullOrWhiteSpace(providerOptions.Data.AuthorizationHeaderScheme))
                {
                    return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, $"{group.Key}.Data.AuthorizationHeaderScheme"));
                }
            }
        }

        return ValidateOptionsResult.Success;
    }
}