using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amica.Models;

namespace Amica.Storage
{
    public interface IBulkRepository : IRepository
    {
        /// <summary>
        /// Asyncronously retrieve all objects of type T. Please note that depending on
        /// the implementation this method might have severe impact on datastore performance,
        /// so use it with caution.
        /// </summary>
        Task<IList<T>> Get<T>() where T : BaseModel;

        /// <summary>
        /// Asyncronously retrieve all objects of type T. Please note that depending on
        /// the implementation this method might have severe impact on datastore performance,
        /// so use it with caution.
        /// </summary>
        /// <param name="companyId">Company Id.</param>
        /// <returns>All objects belonging to company <paramref name="companyId"/>.</returns>
        Task<IList<T>> Get<T>(string companyId) where T : BaseModelWithCompanyId;

        /// <summary>
        /// Asyncronously retrieve all objects which have changed since a certain datetime.
        /// </summary>
        /// <param name="ifModifiedSince">If modified since.</param>
        /// <returns>All objects changed since <paramref name="ifModifiedSince"/></returns>
        Task<IList<T>> Get<T>(DateTime? ifModifiedSince) where T : BaseModel;

        /// <summary>
        /// Asyncronously retrieve all objects which have changed since a certain datetime.
        /// </summary>
        /// <param name="ifModifiedSince">If modified since.</param>
        /// <param name="companyId">Company Id.</param>
        /// <returns>All objects belonging to company <paramref name="companyId"/> which have changed since <paramref name="ifModifiedSince"/></returns>
        Task<IList<T>> Get<T>(DateTime? ifModifiedSince, string companyId) where T : BaseModelWithCompanyId;

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
        /// <returns>The inserted objects.</returns>
        Task<IList<T>> Insert<T>(IEnumerable<T> objs) where T : BaseModel;

        /// <summary>
        /// Asyncronously delete a number of objects. If any object could not be found or deleted,
        /// it will be skipped and no exception will be raised.
        /// </summary>
        /// <typeparam name="T">Type of objects to be deleted.</typeparam>
        /// <param name="objs">Objects to be deleted.</param>
        /// <returns>The unique ids of deleted objects.</returns>
        Task<IList<string>> Delete<T>(IEnumerable<T> objs) where T : BaseModel;

        /// <summary>
        /// Asyncronously delete all objects. Use with caution.
        /// </summary>
        /// <typeparam name="T">Type of objects to be deleted.</typeparam>
        Task Delete<T>() where T : BaseModel;
    }
}