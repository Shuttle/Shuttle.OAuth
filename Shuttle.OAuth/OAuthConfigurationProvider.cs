using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Shuttle.Core.Contract;

namespace Shuttle.OAuth
{
    public class OAuthConfigurationProvider : IOAuthConfigurationProvider
    {
        private readonly Dictionary<string, IOAuthConfiguration> _credentials = new Dictionary<string, IOAuthConfiguration>();
        
        public static IOAuthConfigurationProvider Open(string path)
        {
            Guard.AgainstNullOrEmptyString(path,nameof(path));
            
            if (!File.Exists(path))
            {
                throw new ApplicationException($"Could not locate OAuth configuration path '{path}'.");
            }

            return FromJson(File.ReadAllText(path));
        }

        public static IOAuthConfigurationProvider FromJson(string json)
        {
            var result = new OAuthConfigurationProvider();

            var credentials = JsonConvert.DeserializeObject<List<OAuthConfiguration>>(json);

            if (credentials != null)
            {
                foreach (var credential in credentials)
                {
                    result.Add(credential);
                }
            }

            return result;
        }

        public IOAuthConfiguration Get(string name)
        {
            Guard.AgainstNullOrEmptyString(name,nameof(name));

            if (_credentials.ContainsKey(name))
            {
                return _credentials[name];
            }

            throw new ApplicationException(string.Format(Resources.MissingNameException, name, "OAuth"));
        }

        public void Add(IOAuthConfiguration configuration)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            
            configuration.ApplyInvariants();

            if (_credentials.ContainsKey(configuration.Name))
            {
                throw new ApplicationException(string.Format(Resources.DuplicateNameException, configuration.Name, "OAuth"));
            }

            _credentials.Add(configuration.Name, configuration);
        }
    }
}