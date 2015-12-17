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

        public async Task<T> Get<T>(T obj) where T : BaseModel
        {
            var found = false;

			// 1. retrieve from local 
            try
            {
				obj = await Local.Get(obj);
                found = true;
            }
			catch (LocalObjectNotFoundRepositoryException) { }

            var lastUpdated = obj.Updated;

			// 2. check if remote has updated version and download it
            try
            {
				obj = await Remote.Get(obj);
                found = true;
            }
			catch (RemoteObjectNotFoundRepositoryException) { }

            if (!found)
                throw new StorageServiceObjectNotFoundRepositoryException(obj);

			// 3. if downloaded, store update locally
            if (obj.Updated > lastUpdated)
                await Local.Replace(obj);

            // 3. return object
            return obj;
        }

        public Task<T> Insert<T>(T obj) where T : BaseModel
        {
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
