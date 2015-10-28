using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amica.vNext.Models;

namespace Amica.vNext.Data
{
    public interface IStorage : IDisposable
    {
        /// <summary>
        /// Asyncronoulsy  return a refreshed object from the datastore.
        /// </summary>
        /// <param name="obj">The object to refresh.</param>
        /// <returns>An object from the datastore.</returns>
		/// <exception cref="ObjectNotFoundStorageException"> if <paramref name="obj"/> was not found.</exception>
        Task<T> Get<T>(T obj) where T : BaseModel;

        /// <summary>
        /// Asyncronoulsy insert an object into the datastore.
        /// </summary>
        /// <param name="obj">The object to be stored.</param>	
        /// <returns>The insterted object</returns>
		/// <exception cref="ValidationStorageException">If a validation error was returned by the service.</exception>
        Task<T> Insert<T>(T obj) where T : BaseModel;

        /// <summary>
        /// Asyncronoulsy replaces an object into the the datastore.
        /// </summary>
        /// <param name="obj">The object to be updated.</param>	
		/// <exception cref="ObjectNotFoundStorageException"> if <paramref name="obj"/> was not found.</exception>
		/// <exception cref="PreconditionFailedStorageException">If object ETag did not match the one currently on the service (remote object has been updated in the meanwhile).</exception>
        Task Delete<T>(T obj) where T : BaseModel;

        /// <summary>
        /// Asyncronoulsy replaces an object into the datastore.
        /// </summary>
        /// <param name="obj">The object instance to be stored in the datastore.</param>	
        /// <returns>The replaced object</returns>
		/// <exception cref="ObjectNotFoundStorageException"> if <paramref name="obj"/> was not found.</exception>
		/// <exception cref="PreconditionFailedStorageException">If object ETag did not match the one currently on the service (remote object has been updated in the meanwhile).</exception>
		/// <exception cref="ValidationStorageException">If a validation error was returned by the service.</exception>
        Task<T> Replace<T>(T obj) where T : BaseModel;
    }

    public interface IBulkStorage : IStorage
    {
		/// <summary>
        /// The company id to be used when retrieving objects. If null, no filter on company will be issued.
        /// </summary>
		string CompanyId { get; set; }

        /// <summary>
        /// Asyncronously retrieve all objects of type T. Please note that depending on
        /// the implementation this method might have severe impact on datastore performance,
        /// so use it with caution.
        /// </summary>
        Task<IList<T>> Get<T>();

        /// <summary>
        /// Asyncronously retrieve all objects of type T. Please note that depending on
        /// the implementation this method might have severe impact on datastore performance,
        /// so use it with caution.
        /// </summary>
        /// <param name="companyId">Company Id.</param>
        /// <returns>All objects belonging to company <paramref name="companyId"/>.</returns>
        Task<IList<T>> Get<T>(string companyId);

        /// <summary>
		/// Asyncronously retrieve all objects which have changed since a certain datetime.
		/// </summary>
		/// <param name="ifModifiedSince">If modified since.</param>
		/// <returns>All objects changed since <paramref name="ifModifiedSince"/></returns>
        Task<IList<T>> Get<T>(DateTime? ifModifiedSince);

        /// <summary>
        /// Asyncronously retrieve all objects which have changed since a certain datetime.
        /// </summary>
        /// <param name="ifModifiedSince">If modified since.</param>
        /// <param name="companyId">Company Id.</param>
        /// <returns>All objects belonging to company <paramref name="companyId"/> which have changed since <paramref name="ifModifiedSince"/></returns>
        Task<IList<T>> Get<T>(DateTime? ifModifiedSince, string companyId);

        /// <summary>
        /// Asyncronously get several objects from the datastore.
        /// Eventual missing keys will be ignored and no exception will be raised.
        /// </summary>
        /// <param name="uniqueIds">The ids to look up in the datastore.</param>
        /// <returns>The objects from the datastore.</returns>
        Task<IDictionary<string, T>> Get<T>(IEnumerable<string> uniqueIds) where T : BaseModel, new();

        /// <summary>
        /// Asyncronously insert several objects into the datastore. If one or more
        /// objects are rejected by the service, the whole batch is reject and no
        /// document is stored on the service.
        /// </summary>
        /// <returns>The objects inserted.</returns>
		/// <exception cref="ValidationStorageException">If one or more objects has failed validation.</exception>
        Task<IList<T>> Insert<T>(IEnumerable<T> objs) where T : BaseModel;

		/// <summary>
		/// Asyncronously delete a number of objects. If any object could not be found or deleted,
		/// it will be skipped and no exception will be raised.
		/// </summary>
		/// <typeparam name="T">Type of objects to be deleted.</typeparam>
		/// <param name="objs">Unique ids of the objects to be deted.</param>
		/// <returns>The unique ids of deleted objects.</returns>
        Task<IList<string>> Delete<T>(IEnumerable<T> objs) where T : BaseModel;
    }
}
