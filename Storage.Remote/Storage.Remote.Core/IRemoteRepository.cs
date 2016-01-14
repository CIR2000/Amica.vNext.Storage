using System.Threading.Tasks;

namespace Amica.vNext.Storage
{
    public interface IRemoteRepository : IBulkRepository
    {
		IBulkObjectCache LocalCache { get; set; }
		Discovery DiscoveryService { get; set; }
        Task InvalidateUser(string username);
    }
}
