﻿using System.Collections.Generic;
using Shuttle.Core.Contract;

namespace Shuttle.OAuth
{
    public class OAuthProviderService : IOAuthProviderService
    {
        private readonly Dictionary<string, IOAuthProvider> _providers = new();
        
        public OAuthProviderService(IEnumerable<IOAuthProvider> providers)
        {
            foreach (var provider in providers)
            {
                var key = provider.Name.ToLowerInvariant();
                
                if (_providers.ContainsKey(key))
                {
                    throw new OAuthException(string.Format(Resources.MissingProviderException, provider.Name));
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
                throw new OAuthException(string.Format(Resources.MissingProviderException, name));
            }

            return _providers[key];
        }
    }
}