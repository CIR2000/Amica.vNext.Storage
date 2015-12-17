using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amica.vNext.Models;

namespace Amica.vNext.Storage
{
    public class StorageService : ILocalBulkRepository, IRemoteRepository
    {
        public string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<T> Get<T>(T obj) where T : BaseModel
        {
            throw new NotImplementedException();
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

        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
    }
}
