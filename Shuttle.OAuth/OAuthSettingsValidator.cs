using Microsoft.Extensions.Options;

namespace Shuttle.OAuth
{
    public class OAuthOptionsValidator : IValidateOptions<OAuthOptions>
    {
        public ValidateOptionsResult Validate(string? name, OAuthOptions options)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return ValidateOptionsResult.Fail(Resources.OptionsNameRequired);
            }

            if (string.IsNullOrWhiteSpace(options.ClientId))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, "ClientId"));
            }

            if (string.IsNullOrWhiteSpace(options.ClientSecret))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, "ClientSecret"));
            }

            if (string.IsNullOrWhiteSpace(options.TokenUrl))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, "TokenUrl"));
            }

            if (string.IsNullOrWhiteSpace(options.DataUrl))
            {
                return ValidateOptionsResult.Fail(string.Format(Resources.OptionRequired, "DataUrl"));
            }
            
            return ValidateOptionsResult.Success;
        }
    }
}