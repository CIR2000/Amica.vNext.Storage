using System;
using System.Threading.Tasks;
using Storage.Remote;

namespace Amica.vNext.Storage
{
    public interface IRemoteRepository: IBulkRepositoryRemote
    {
        string Username { get; set; }
        string Password { get; set; }
        string ClientId { get; set; }
		Uri DiscoveryUri { get; set; }
		IBulkObjectCache LocalCache { get; set; }
        Discovery DiscoveryService { get; }
        Task InvalidateUser(string username);
    }
}
