using System;
using System.Collections.Generic;
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
            await SyncRemoteWithLocal<T>();
            return await Local.Get<T>();
        }

        /// <summary>
        /// Asyncronously retrieve all objects of type T. This implementation uses an
        /// internal cache to provide optimum performance, but can still heavy on large
        /// datasets.
		/// </summary>
        /// <param name="companyId">Company Id.</param>
        /// <returns>All objects belonging to company <paramref name="companyId"/>.</returns>
        public async Task<IList<T>> Get<T>(string companyId) where T : BaseModelWithCompanyId
        {
            await SyncRemoteWithLocal<T>(null, companyId);
            return await Local.Get<T>(companyId);
        }

        /// <summary>
        /// Asyncronously retrieve all objects which have changed since a certain datetime.
        /// </summary>
        /// <param name="ifModifiedSince">If modified since.</param>
        /// <returns>All objects changed since <paramref name="ifModifiedSince"/></returns>
        public async Task<IList<T>> Get<T>(DateTime? ifModifiedSince) where T : BaseModel
        {
            await SyncRemoteWithLocal<T>(ifModifiedSince);
            return await Local.Get<T>(ifModifiedSince);
        }

        /// <summary>
        /// Asyncronously retrieve all objects which have changed since a certain datetime.
        /// </summary>
        /// <param name="ifModifiedSince">If modified since.</param>
        /// <param name="companyId">Company Id.</param>
        /// <returns>All objects belonging to company <paramref name="companyId"/> which have changed since <paramref name="ifModifiedSince"/></returns>
        public async Task<IList<T>> Get<T>(DateTime? ifModifiedSince, string companyId) where T : BaseModelWithCompanyId
        {
            await SyncRemoteWithLocal<T>(ifModifiedSince, companyId);
            return await Local.Get<T>(ifModifiedSince, companyId);
        }

        /// <summary>
        /// Asyncronously get several objects from the datastore.
        /// Eventual missing keys will be ignored and no exception will be raised.
        /// </summary>
        /// <param name="uniqueIds">The ids to look up in the datastore.</param>
        /// <returns>The objects from the datastore.</returns>
        public async Task<IDictionary<string, T>> Get<T>(IEnumerable<string> uniqueIds) where T : BaseModel, new()
        {
            await SyncRemoteWithLocal<T>();
            return await Local.Get<T>(uniqueIds);
        }

        /// <summary>
        /// Asyncronously insert several objects into the datastore. If one or more
        /// objects are rejected by the service, the whole batch is reject and no
        /// document is stored on the service.
        /// </summary>
        /// <returns>The inserted objects.</returns>
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

        /// <summary>
        /// Syncronizes remote and local storages.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="ifModifiedSince">If modified since.</param>
        /// <returns></returns>
        private async Task SyncRemoteWithLocal<T>(DateTime? ifModifiedSince = null) where T : BaseModel
        {
			var remotes = await Remote.Get<T>(await OptimizedIfModifiedSince<T>(ifModifiedSince));
            await UpdateLocalWithRemotes(remotes);
        }

        /// <summary>
        /// Syncronizes remote and local storages.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="ifModifiedSince">If modified since.</param>
        /// <param name="companyId">Company Id.</param>
        /// <returns></returns>
        private async Task SyncRemoteWithLocal<T>(DateTime? ifModifiedSince, string companyId) where T : BaseModelWithCompanyId
        {
			var remotes = await Remote.Get<T>(await OptimizedIfModifiedSince<T>(ifModifiedSince), companyId);
            await UpdateLocalWithRemotes(remotes);
        }

		/// <summary>
        /// Will usually return the datetime of the last modified local object.
        /// <paramref name="ifModifiedSince"/> is returned if it predates last modified object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ifModifiedSince"></param>
        /// <returns>The optimal If-Modified-Since value (a DateTime).</returns>
        private async Task<DateTime> OptimizedIfModifiedSince<T>(DateTime? ifModifiedSince) where T : BaseModel
       {
			var lastModified = await Local.LastModified<T>();
            return ifModifiedSince != null && ifModifiedSince < lastModified ? (DateTime)ifModifiedSince : lastModified;
            
        }

		/// <summary>
        /// Updates local storage with upstream changes.
        /// </summary>
        /// <typeparam name="T">Objects type.</typeparam>
        /// <param name="remotes">Upstream changes.</param>
        private async Task UpdateLocalWithRemotes<T>(IEnumerable<T> remotes) where T : BaseModel
        {
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
