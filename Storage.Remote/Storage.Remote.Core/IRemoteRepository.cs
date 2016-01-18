using System;
using System.Threading.Tasks;
using Storage.Remote;

namespace Amica.vNext.Storage
{
    public interface IRemoteRepository: IBulkRepositoryRemote
    {
		/// <summary>
        /// Logged in user or null.
        /// </summary>
		UserAccount UserAccount { get; set; }
        Task<bool> Login(bool persist);
        Task<bool> Login(UserAccount account, bool persist);
        Task Logout();
		/// <summary>
		/// Used to identify the client against the authentications service. 
		/// </summary>
		string ClientId { get; set; }
		Uri DiscoveryUri { get; set; }
		IBulkObjectCache LocalCache { get; set; }
        Discovery DiscoveryService { get; }
        Task InvalidateUser(string username);
        Task SaveOrInvalidateAccount(bool persist);
    }
}
