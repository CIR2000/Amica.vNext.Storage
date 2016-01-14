using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amica.vNext.Models;

namespace Amica.vNext.Storage
{
    public interface ILocalBulkRepository : IBulkRepository, ILocalRepository
    {
		/// <summary>
        /// Asyncronoulsy returns  the last datetime at which the collection has been modified.
        /// </summary>
        /// <typeparam name="T">Type of object collection to investigate.</typeparam>
        /// <returns>A DateTime expressing the last time the collection has been modified.</returns>
        Task<DateTime> LastModified<T>() where T : BaseModel;
        Task InsertOrReplace<T>(IEnumerable<T> objs) where T : BaseModel;
    }
}