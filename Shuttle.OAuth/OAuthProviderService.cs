using System.Collections;
using System.Collections.Generic;
using Shuttle.Core.Contract;

namespace Shuttle.OAuth
{
    public class OAuthProviderService : IOAuthProviderService
    {
        private readonly Dictionary<string, IOAuthProvider> _providers = new Dictionary<string, IOAuthProvider>();
        
        public OAuthProviderService(IEnumerable<IOAuthProvider> providers)
        {
            Guard.AgainstNull(providers, nameof(providers));

            foreach (var provider in providers)
            {
                var key = provider.Name.ToLowerInvariant();
                
                if (_providers.ContainsKey(key))
                {
                    throw new OAuthException(string.Format(Resources.DuplicateNameException, provider.Name,
                        "OAuthProvider"));
                }
                
                _providers.Add(key, provider);
            }
        }

        public IOAuthProvider Get(string name)
        {
            Guard.AgainstNullOrEmptyString(name,nameof(name));

            var key = name.ToLowerInvariant();
            
            if (!_providers.ContainsKey(key))
            {
                throw new OAuthException(string.Format(Resources.MissingNameException, name, "OAuthProvider"));
            }

            return _providers[key];
        }
    }
}