namespace Shuttle.OAuth
{
    public interface IOAuthProviderCollection
    {
        IOAuthProvider Get(string name);
    }
}