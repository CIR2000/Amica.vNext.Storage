using System;
using System.Collections.Generic;
using Amica.Models;

namespace Amica.Storage
{
    public interface IBulkRepositoryRemote : IBulkRepository
    {
        /// <summary>
        /// Merge two lists of objects, overwriting the original objects
        /// with fresh content when available.
        /// </summary>
        IList<T> Merge<T>(IList<T> originalContent, IList<T> newContent) where T : BaseModel;
        /// <summary>
        /// Last valid 'Last-Modified' header value returned, if available.
        /// </summary>
        DateTime? LastModifiedResponseHeader { get; set; }
    }
}
