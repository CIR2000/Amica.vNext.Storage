using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amica.vNext.Models;

namespace Amica.vNext.Storage
{
    public class StorageService : ILocalBulkRepository, IRemoteRepository
    {
        private static readonly LocalRepository Local = new LocalRepository();
        private static readonly RemoteRepository Remote = new RemoteRepository();

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
            // 1. attempt to insert obj remotely
			// 2. if not successfull, report and exit
			// 3. insert locally
			// 4. return object (now with fresh metadata)
            throw new NotImplementedException();
        }

        public Task Delete<T>(T obj) where T : BaseModel
        {
            throw new NotImplementedException();
        }

        public Task<T> Replace<T>(T obj) where T : BaseModel
        {
            throw new NotImplementedException();
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
