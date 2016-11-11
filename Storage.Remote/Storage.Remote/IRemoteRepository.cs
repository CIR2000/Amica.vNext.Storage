using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amica.Models;
using SimpleObjectCache;

namespace Amica.Storage
{
    public interface IRemoteRepository: IBulkRepositoryRemote, IRestoreDefaults
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
        Discovery.Discovery DiscoveryService { get; }
        Task InvalidateUser(string username);
        Task SaveOrInvalidateAccount(bool persist);
        /// <summary>
        /// Merge two lists of objects, overwriting the original objects
        /// with fresh content when available.
        /// </summary>
        IList<T> Merge<T>(IList<T> originalContent, IList<T> newContent) where T : BaseModel;
    }
}
