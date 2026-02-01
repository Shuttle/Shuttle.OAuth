using Shuttle.Core.Contract;

namespace Shuttle.OAuth;

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
        if (!_grants.TryGetValue(id, out var grant))
        {
            throw new OAuthException(string.Format(Resources.OAuthGrantNotFoundException, id));
        }

        return await Task.FromResult(grant);
    }
}