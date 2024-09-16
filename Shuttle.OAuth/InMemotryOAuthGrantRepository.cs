using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.OAuth
{
    public class InMemoryOAuthGrantRepository : IOAuthGrantRepository
    {
        private readonly Dictionary<Guid, OAuthGrant> _grants = new();

        public async Task SaveAsync(OAuthGrant grant)
        {
            Guard.AgainstNull(grant);

            _grants[grant.Id] = grant;

            await Task.CompletedTask;
        }

        public async Task<OAuthGrant> GetAsync(Guid id)
        {
            if (!_grants.ContainsKey(id))
            {
                throw new OAuthException(string.Format(Resources.OAuthGrantNotFoundException, id));
            }

            return await Task.FromResult(_grants[id]);
        }
    }
}