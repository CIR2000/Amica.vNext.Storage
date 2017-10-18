using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amica.Models;

namespace Amica.Storage
{
    public interface ICompanyRepository : IBulkRepository
    {
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
        /// <param name="companyId">Company Id.</param>
        /// <returns>All objects belonging to company <paramref name="companyId"/> which have changed since <paramref name="ifModifiedSince"/></returns>
        Task<IList<T>> Get<T>(DateTime? ifModifiedSince, string companyId) where T : BaseModelWithCompanyId;

        /// <summary>
        /// Asyncronously retrieve all objects which have changed since a certain datetime.
        /// </summary>
        /// <param name="ifModifiedSince">If modified since.</param>
        /// <param name="companyId">Company Id.</param>
        /// <param name="softDeleted">Wether deleted objects should be included or not.</param>
        /// <returns>All objects belonging to company <paramref name="companyId"/> which have changed since <paramref name="ifModifiedSince"/></returns>
        Task<IList<T>> Get<T>(DateTime? ifModifiedSince, string companyId, bool softDeleted) where T : BaseModelWithCompanyId;
    }
}