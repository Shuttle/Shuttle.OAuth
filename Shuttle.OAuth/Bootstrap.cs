using System.Collections;
using System.Collections.Generic;
using Shuttle.Core.Container;
using Shuttle.Core.Contract;
using Shuttle.Core.Reflection;

namespace Shuttle.OAuth
{
    public class Bootstrap : IComponentRegistryBootstrap
    {
        private static bool _initialized;
        private static readonly object Lock = new object();
        
        public void Register(IComponentRegistry registry)
        {
            Guard.AgainstNull(registry, nameof(registry));

            lock (Lock)
            {
                if (_initialized)
                {
                    return;
                }

                registry.RegisterCollection(typeof(IOAuthProvider), new ReflectionService().GetTypesAssignableTo<IOAuthProvider>(), Lifestyle.Singleton);
                registry.AttemptRegister<IOAuthProviderCollection, OAuthProviderCollection>();

                _initialized = true;
            }
        }
    }
}