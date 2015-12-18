using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Amica.vNext.Models;

[assembly:InternalsVisibleTo("Storage.Service.Tests")]

namespace Amica.vNext.Storage
{
    public class StorageService : ILocalBulkRepository, IRemoteRepository
    {
        internal static readonly LocalRepository Local = new LocalRepository();
        internal static readonly RemoteRepository Remote = new RemoteRepository();

        public void Dispose()
        {
            Local.Dispose();
            Remote.Dispose();
        }

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

			// TODO pick a behavior when remote does not have the object
			// while local does. Delete from local and raise not found,
			// or return th elocal we we are doing now?

			// TODO we should be using the show_deleted option, download
			// the object and, if it is deleted, act accordingly.

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

        public Task<IList<T>> Get<T>() where T : BaseModel
        {
            throw new NotImplementedException();
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

        public Task<IList<T>> Insert<T>(IEnumerable<T> objs) where T : BaseModel
        {
            throw new NotImplementedException();
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
