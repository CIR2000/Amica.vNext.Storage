using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Amica.vNext.Models;

[assembly:InternalsVisibleTo("Storage.Service.Tests")]

namespace Amica.vNext.Storage
{
    public class StorageService :  IRemoteRepository
    {
        internal readonly LocalRepository Local = new LocalRepository();
        internal readonly RemoteRepository Remote = new RemoteRepository();

        public void Dispose()
        {
            Local.Dispose();
            Remote.Dispose();
        }

        #region "IRepository"

        /// <summary>
        /// Asyncronoulsy  return a refreshed object from the datastore.
        /// </summary>
        /// <param name="obj">The object to refresh.</param>
        /// <returns>An object from the datastore.</returns>
        /// <exception cref="RemoteObjectNotFoundStorageException"> if <paramref name="obj"/> was not found.</exception>
        public async Task<T> Get<T>(T obj) where T : BaseModel
        {
            try
            {
				obj = await Local.Get(obj);
            }
			catch (LocalObjectNotFoundStorageException) { }

            var lastUpdated = obj.Updated;

			// TODO if remote has been deleted, delete from local and raise an exception
			// this will require updating Eve.NET to properly handlse soft deltes

			// Will eventually throw RemoteObjectNotFoundException.
			obj = await Remote.Get(obj);

            if (obj.Updated > lastUpdated)
                await Local.Replace(obj);

            return obj;
        }

        /// <summary>
        /// Asyncronoulsy insert an object into the datastore.
        /// </summary>
        /// <param name="obj">The object to be stored.</param>	
        /// <returns>The insterted object</returns>
		/// <exception cref="ValidationStorageException">If a validation error was returned by the service.</exception>
        public async Task<T> Insert<T>(T obj) where T : BaseModel
        {
            obj = await Remote.Insert(obj);
            await Local.Insert(obj);
            return obj;
        }

        /// <summary>
        /// Asyncronoulsy replaces an object into the the datastore.
        /// </summary>
        /// <param name="obj">The object to be updated.</param>	
		/// <exception cref="RemoteObjectNotFoundStorageException"> if <paramref name="obj"/> was not found.</exception>
		/// <exception cref="PreconditionFailedStorageException">If object ETag did not match the one currently on the service (remote object has been updated in the meanwhile).</exception>
        public async Task Delete<T>(T obj) where T : BaseModel
        {
            await Remote.Delete(obj);
            try
            {
                await Local.Delete(obj);
            }
            catch (LocalObjectNotDeletedStorageException)
            {
				// TODO log this exception? Maybe throw it? Should never happen.
            }
        }

        /// <summary>
        /// Asyncronoulsy replaces an object into the datastore.
        /// </summary>
        /// <param name="obj">The object instance to be stored in the datastore.</param>	
        /// <returns>The replaced object</returns>
		/// <exception cref="RemoteObjectNotFoundStorageException"> if <paramref name="obj"/> was not found.</exception>
		/// <exception cref="PreconditionFailedStorageException">If object ETag did not match the one currently on the service (remote object has been updated in the meanwhile).</exception>
		/// <exception cref="ValidationStorageException">If a validation error was returned by the service.</exception>
        public async Task<T> Replace<T>(T obj) where T : BaseModel
        {
            obj = await Remote.Replace(obj);
            try
            {
                obj = await Local.Replace(obj);
            }
            catch (LocalObjectNotReplacedStorageException)
            {
				// TODO log this exception? Maybe throw it? Should never happen.
            }
            return obj;
        }
        #endregion

        #region "IBulkRepository"

        /// <summary>
        /// Asyncronously retrieve all objects of type T. This implementation uses an
        /// internal cache to provide optimum performance, but can still heavy on large
        /// datasets.
        /// </summary>
        public async Task<IList<T>> Get<T>() where T : BaseModel
        {
			var lastModified = await Local.LastModified<T>();
            var remotes = await Remote.Get<T>(lastModified);

            var toInsertOrReplace = new List<T>();
            var toDelete = new List<T>();

            foreach (var obj in remotes)
            {
                if (obj.Deleted)
                    toDelete.Add(obj);
                else
                    toInsertOrReplace.Add(obj);
            }
            await Local.Delete<T>(toDelete);
            await Local.InsertOrReplace(toInsertOrReplace);

            return await Local.Get<T>();
        }

        public Task<IList<T>> Get<T>(string companyId) where T : BaseModelWithCompanyId
        {
            throw new NotImplementedException();
        }

        public Task<IList<T>> Get<T>(DateTime? ifModifiedSince) where T : BaseModel
        {
            throw new NotImplementedException();
        }

        public Task<IList<T>> Get<T>(DateTime? ifModifiedSince, string companyId) where T : BaseModelWithCompanyId
        {
            throw new NotImplementedException();
        }

        public Task<IDictionary<string, T>> Get<T>(IEnumerable<string> uniqueIds) where T : BaseModel, new()
        {
            throw new NotImplementedException();
        }

        public async Task<IList<T>> Insert<T>(IEnumerable<T> objs) where T : BaseModel
        {
            return await Local.Insert<T>(await Remote.Insert(objs));
        }

        public Task<IList<string>> Delete<T>(IEnumerable<T> objs) where T : BaseModel
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asyncronously delete all objects. Use with caution.
        /// </summary>
        /// <typeparam name="T">Type of objects to be deleted.</typeparam>
        public async Task Delete<T>() where T : BaseModel
        {
            await Remote.Delete<T>();
            await Local.Delete<T>();
        }

        #endregion

        #region "Properties"
        public string Username
        {
            get { return Remote.Username; }
            set { Remote.Username = value; }
        }

        public string Password
        {
            get { return Remote.Password; }
            set { Remote.Password = value; }
        }

        public string ClientId
        {
            get { return Remote.ClientId; }
            set { Remote.ClientId = value; }
        }
        public string ApplicationName
        {
            get { return Local.ApplicationName; }
            set { Local.ApplicationName = value; }
        }
        #endregion
    }
}
