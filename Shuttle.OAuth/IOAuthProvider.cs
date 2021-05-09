namespace Shuttle.OAuth
{
    public interface IOAuthProvider
    {
        string Name { get; }
        dynamic GetData(string code);
    }
}