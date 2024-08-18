using System.Threading.Tasks;

namespace Shuttle.OAuth
{
    public interface IOAuthProvider
    {
        string Name { get; }
        Task<dynamic?> GetDataDynamicAsync(string code);
    }
}