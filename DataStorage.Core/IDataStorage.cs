using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amica.vNext.Data
{
    public interface IDataStorage : IDisposable
    {
        /// <summary>
        /// Asynctronoulsy retrieve an object from the datastore.
        /// </summary>
        /// <param name="uniqueId">The id of the object to retrieve.</param>
        /// <returns>An object from the datastore. </returns>
	/// <exception cref="ObjectNotFoundException"> if the <paramref name="uniqueId"/> was not found.</exception>
        Task<T> Get<T>(string uniqueId);

        /// <summary>
        /// Asynctronoulsy return a refreshed object from the datastore.
        /// </summary>
        /// <param name="obj">The object to refresh.</param>
        /// <returns>An object from the datastore.</returns>
	/// <exception cref="ObjectNotFoundException"> if <paramref name="obj"/> was not found.</exception>
        Task<T> Get<T>(T obj);

        /// <summary>
        /// Insert an object into the datastore.
        /// </summary>
        /// <param name="obj">The object to be stored.</param>	
	/// <returns>The insterted object</returns>
        Task<T> Insert<T>(T obj);

        /// <summary>
        /// Replaces an object into the the datastore.
        /// </summary>
        /// <param name="obj">The object to be updated.</param>	
	/// <returns>The number of deleted objects.</returns>
        Task<int> Delete<T>(T obj);

        /// <summary>
        /// Replaces an object into the datastore.
        /// </summary>
        /// <param name="obj">The object instance to be stored in the datastore.</param>	
	/// <returns>The replaced object</returns>
        Task Replace<T>(T obj);
    }

    public interface IBulkDataStorage : IDataStorage
    {
        /// <summary>
        /// Asyncronously retrieve all objects of type T. Please note that depending on
        /// the implementation this method might have severe impact on datastore performance,
        /// so use it with caution.
        /// </summary>
        Task<IEnumerable<T>> Get<T>();

        /// <summary>
	///  Asyncronously retrieve all objects which have changed since a certain datetime.
	/// </summary>
	/// <param name="ifModifiedSince">If modified since.</param>
	/// <returns>All objects changed since <paramref name="ifModifiedSince"/></returns>
        Task<IEnumerable<T>> Get<T>(DateTimeOffset ifModifiedSince);

        /// <summary>
        /// Get several objects from the datastore.
        /// Eventual missing keys will be ignored and no exception will be raised.
        /// </summary>
        /// <param name="uniqueIds">The ids to look up in the datastore.</param>
        /// <returns>The objects from the datastore.</returns>
        Task<IDictionary<string, T>> Get<T>(IEnumerable<string> uniqueIds);

        /// <summary>
        /// Insert several objects into the datastore. 
        /// </summary>
        /// <returns>The objects inserted.</returns>
	/// <exception cref="ValidationException">If one or more objects has failed validation.</exception>
	// TODO more exceptions?
        Task<IEnumerable<T>> Insert<T>(IEnumerable<T> objs);

	/// <summary>
	/// Asyncronously delete a number of objects.
	/// </summary>
	/// <typeparam name="T">Type of objects to be deleted.</typeparam>
	/// <param name="uniqueIds">Unique ids of the objects to be deted.</param>
	/// <returns>The number of deleted objects.</returns>
	/// <exception cref="ObjectNotFoundException">If an object was not found.</exception>"
        Task<int> Delete<T>(IEnumerable<T> uniqueIds);
    }
}
