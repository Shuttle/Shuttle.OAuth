using System;
using Shuttle.Core.Contract;

namespace Shuttle.OAuth
{
    public class OAuthGrant
    {
        public OAuthGrant(Guid id, string providerName)
        {
            Id = id;
            ProviderName = Guard.AgainstNullOrEmptyString(providerName);
        }

        public Guid Id { get; }
        public string CodeChallenge { get; private set; } = default!;
        public string CodeVerifier { get; private set; } = default!;
        public string ProviderName { get; }

        public OAuthGrant WithCodeChallenge(string codeChallenge, string codeVerifier)
        {
            CodeChallenge = Guard.AgainstNullOrEmptyString(codeChallenge);
            CodeVerifier = Guard.AgainstNullOrEmptyString(codeVerifier);

            return this;
        }
    }
}