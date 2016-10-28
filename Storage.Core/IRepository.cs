using System;
using System.Threading.Tasks;
using Amica.Models;

namespace Amica.vNext.Storage
{
    public interface IRepository : IDisposable
    {
        /// <summary>
        /// Asyncronoulsy  return a refreshed object from the datastore.
        /// </summary>
        /// <param name="obj">The object to refresh.</param>
        /// <returns>An object from the datastore.</returns>
        Task<T> Get<T>(T obj) where T : BaseModel;

        /// <summary>
        /// Asyncronoulsy insert an object into the datastore.
        /// </summary>
        /// <param name="obj">The object to be stored.</param>	
        /// <returns>The insterted object</returns>
        Task<T> Insert<T>(T obj) where T : BaseModel;

        /// <summary>
        /// Asyncronoulsy replaces an object into the the datastore.
        /// </summary>
        /// <param name="obj">The object to be updated.</param>	
        Task Delete<T>(T obj) where T : BaseModel;

        /// <summary>
        /// Asyncronoulsy replaces an object into the datastore.
        /// </summary>
        /// <param name="obj">The object instance to be stored in the datastore.</param>	
        /// <returns>The replaced object</returns>
        Task<T> Replace<T>(T obj) where T : BaseModel;
    }
}
