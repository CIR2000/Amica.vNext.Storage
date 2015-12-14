using System;
using System.Threading.Tasks;
using Amica.vNext.Models;

namespace Amica.vNext.Storage
{
    class LocalRepository : ILocalRepository
    {
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

        public string DatabasePath { get; set; }
    }
}
