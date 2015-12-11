using System;
using System.Threading.Tasks;
using Amica.vNext.Models;

namespace Amica.vNext.Storage
{
    public interface IRepository : IDisposable
    {
        /// <summary>
        /// Asyncronoulsy  return a refreshed object from the datastore.
        /// </summary>
        /// <param name="obj">The object to refresh.</param>
        /// <returns>An object from the datastore.</returns>
		/// <exception cref="ObjectNotFoundRepositoryException"> if <paramref name="obj"/> was not found.</exception>
        Task<T> Get<T>(T obj) where T : BaseModel;

        /// <summary>
        /// Asyncronoulsy insert an object into the datastore.
        /// </summary>
        /// <param name="obj">The object to be stored.</param>	
        /// <returns>The insterted object</returns>
		/// <exception cref="ValidationRepositoryException">If a validation error was returned by the service.</exception>
        Task<T> Insert<T>(T obj) where T : BaseModel;

        /// <summary>
        /// Asyncronoulsy replaces an object into the the datastore.
        /// </summary>
        /// <param name="obj">The object to be updated.</param>	
		/// <exception cref="ObjectNotFoundRepositoryException"> if <paramref name="obj"/> was not found.</exception>
		/// <exception cref="PreconditionFailedRepositoryException">If object ETag did not match the one currently on the service (remote object has been updated in the meanwhile).</exception>
        Task Delete<T>(T obj) where T : BaseModel;

        /// <summary>
        /// Asyncronoulsy replaces an object into the datastore.
        /// </summary>
        /// <param name="obj">The object instance to be stored in the datastore.</param>	
        /// <returns>The replaced object</returns>
		/// <exception cref="ObjectNotFoundRepositoryException"> if <paramref name="obj"/> was not found.</exception>
		/// <exception cref="PreconditionFailedRepositoryException">If object ETag did not match the one currently on the service (remote object has been updated in the meanwhile).</exception>
		/// <exception cref="ValidationRepositoryException">If a validation error was returned by the service.</exception>
        Task<T> Replace<T>(T obj) where T : BaseModel;
    }
}
