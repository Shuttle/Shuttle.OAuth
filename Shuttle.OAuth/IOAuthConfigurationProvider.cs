namespace Shuttle.OAuth
{
    public interface IOAuthConfigurationProvider
    {
        IOAuthConfiguration Get(string name);
    }
}