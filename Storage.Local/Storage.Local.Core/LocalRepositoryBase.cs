using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amica.vNext.Models;
using SQLite.Net.Async;
using SQLite.Net;

namespace Amica.vNext.Storage
{
    public abstract class LocalRepositoryBase : ILocalBulkRepository
    {

        protected SQLiteConnectionWithLock LockedConnection;
        private SQLiteAsyncConnection _connection;
		private string _repositoryDirectory;

        protected LocalRepositoryBase()
        {
            RepositoryFileName = "repository.db3";
            ApplicationName = "LocalRepositoryDefaultApplication";
        }
        public void Dispose()
        {
            LockedConnection?.Dispose();
            _connection = null;
        }

        public async Task<T> Get<T>(T obj) where T : BaseModel
        {
            var conn = await Connection();

            var result = await conn.FindAsync<T>(obj.UniqueId);
            if (result == null)
                throw new ObjectNotFoundRepositoryException(obj);

            return result;
        }

        public async Task<T> Insert<T>(T obj) where T : BaseModel
        {
            var conn = await Connection();
            await conn.InsertAsync(obj);
            return obj;
        }

        public async Task Delete<T>(T obj) where T : BaseModel
        {
            var conn = await Connection();
            var i = await conn.DeleteAsync(obj);
            if (i == 0)
				// TODO should fail silently? Or raise DeleteException since
				// we are not sure about the real failure?
                throw new ObjectNotDeletedRepositoryException(obj);
        }

        public async Task<T> Replace<T>(T obj) where T : BaseModel
        {
            var conn = await Connection();

            var i = await conn.UpdateAsync(obj);
            if (i == 0)
                throw new ObjectNotReplacedRepositoryException(obj);

            return obj;
        }

        public async Task<IList<T>> Get<T>() where T : BaseModel
        {
            var conn = await Connection();

            return await conn.Table<T>().ToListAsync();
        }

        public async Task<IList<T>> Get<T>(string companyId) where T : BaseModelWithCompanyId
        {
            var conn = await Connection();

            return await conn.Table<T>().Where(v => v.CompanyId.Equals(companyId)).ToListAsync();
        }

        public async Task<IList<T>> Get<T>(DateTime? ifModifiedSince) where T : BaseModel
        {
            var conn = await Connection();

            return await conn.Table<T>().Where(v => v.Updated > ifModifiedSince).ToListAsync();
        }

        public async Task<IList<T>> Get<T>(DateTime? ifModifiedSince, string companyId) where T : BaseModelWithCompanyId
        {
            var conn = await Connection();

            return await conn.Table<T>().Where(v => v.Updated > ifModifiedSince && v.CompanyId.Equals(companyId)).ToListAsync();
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
        /// Returns the appropriate platform connection.
        /// </summary>
        /// <returns>The platform connection.</returns>
        protected abstract SQLiteAsyncConnection PlatformConnection();

		public string RepositoryDirectory {
		    get
		    {
		        if (_repositoryDirectory != null) return _repositoryDirectory;

		        RepositoryDirectory = DefaultRepositoryDirectory();
		        return _repositoryDirectory;
		    }
		    set
		    {
		        _repositoryDirectory = value;
		    }
        }

		public string RepositoryFileName { get; set; }
        public string RepositoryFullPath => Path.Combine(RepositoryDirectory, RepositoryFileName);

        private string DefaultRepositoryDirectory()
        {
			if (ApplicationName == null)
				throw new ArgumentNullException(nameof(ApplicationName));

			return Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
				Path.Combine(ApplicationName, "LocalRepository"));
        }

        private async Task<SQLiteAsyncConnection> Connection()
        {
            if (_connection != null) return _connection;

            Directory.CreateDirectory(RepositoryDirectory);

            _connection = PlatformConnection();

            await _connection.CreateTableAsync<Company>();
            await _connection.CreateTableAsync<Country>();

            return _connection;
        }

        public string CompanyId { get; set; }

        public string ApplicationName { get; set;}

    }
}
