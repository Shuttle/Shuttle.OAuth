namespace Shuttle.OAuth
{
    public interface IOAuthConfiguration
    {
        string Name { get; }
        string ClientId { get; }
        string ClientSecret { get; }
        string TokenUrl { get; }
        string DataUrl { get; }
    }
}