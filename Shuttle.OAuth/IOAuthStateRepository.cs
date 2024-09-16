using System;
using System.Threading.Tasks;

namespace Shuttle.OAuth
{
    public interface IOAuthGrantRepository
    {
        Task SaveAsync(OAuthGrant grant);
        Task<OAuthGrant> GetAsync(Guid id);
    }
}