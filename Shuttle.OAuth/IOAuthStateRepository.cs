using System;
using System.Threading.Tasks;

namespace Shuttle.OAuth;

public interface IOAuthGrantRepository
{
    Task<OAuthGrant> GetAsync(Guid id);
    Task SaveAsync(OAuthGrant grant);
}