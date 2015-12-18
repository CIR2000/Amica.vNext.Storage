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

        private SQLiteConnectionWithLock _lockedConnection;
        private SQLiteAsyncConnection _connection;
		private string _repositoryDirectory;

        protected LocalRepositoryBase()
        {
            RepositoryFileName = "repository.db3";
            ApplicationName = "LocalRepositoryDefaultApplication";
        }
        public void Dispose()
        {
            _lockedConnection?.Dispose();
            _connection = null;
        }

#region "IRepository"

        /// <summary>
        /// Asyncronoulsy  return a refreshed object from the datastore.
        /// </summary>
        /// <param name="obj">The object to refresh.</param>
        /// <returns>An object from the datastore.</returns>
        /// <exception cref="LocalObjectNotFoundStorageException"> if <paramref name="obj"/> was not found.</exception>
        public async Task<T> Get<T>(T obj) where T : BaseModel
        {
            var conn = await Connection();

            var result = await conn.FindAsync<T>(obj.UniqueId);
            if (result == null)
                throw new LocalObjectNotFoundStorageException(obj);

            return result;
        }

        public async Task<T> Insert<T>(T obj) where T : BaseModel
        {
            var conn = await Connection();
            await conn.InsertAsync(obj);
            return obj;
        }

        /// <summary>
        /// Asyncronoulsy replaces an object into the the datastore.
        /// </summary>
        /// <param name="obj">The object to be updated.</param>	
		/// <exception cref="LocalObjectNotDeletedStorageException"> if <paramref name="obj"/> was not found.</exception>
        public async Task Delete<T>(T obj) where T : BaseModel
        {
            var conn = await Connection();
            var i = await conn.DeleteAsync(obj);
            if (i == 0)
				// TODO should fail silently? Or raise DeleteException since
				// we are not sure about the real failure?
                throw new LocalObjectNotDeletedStorageException(obj);
        }

        public async Task<T> Replace<T>(T obj) where T : BaseModel
        {
            var conn = await Connection();

            var i = await conn.UpdateAsync(obj);
            if (i == 0)
				// TODO should fail silently? Or raise DeleteException since
				// we are not sure about the real failure?
                throw new LocalObjectNotReplacedStorageException(obj);

            return obj;
        }

        #endregion

#region "IBulkRepository"

        public async Task<IList<T>> Get<T>() where T : BaseModel
        {
            var conn = await Connection();

            return await conn.Table<T>().ToListAsync();
        }

        public async Task<IList<T>> Get<T>(DateTime? ifModifiedSince) where T : BaseModel
        {
            var conn = await Connection();

            return await conn.Table<T>().Where(v => v.Updated > ifModifiedSince).ToListAsync();
        }

        public async Task<IList<T>> Get<T>(string companyId) where T : BaseModelWithCompanyId
        {
            var conn = await Connection();

            return await conn.Table<T>().Where(v => v.CompanyId.Equals(companyId)).ToListAsync();
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
        /// Asyncronously delete all objects. Use with caution.
        /// </summary>
        /// <typeparam name="T">Type of objects to be deleted.</typeparam>
        public async Task Delete<T>() where T : BaseModel
        {
            var conn = await Connection();
            await conn.DeleteAllAsync<T>();
        }

        #endregion

        /// <summary>
        /// Returns the appropriate platform connection.
        /// </summary>
        /// <returns>The platform connection.</returns>
        //protected abstract SQLiteAsyncConnection PlatformConnection();

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

        protected abstract SQLiteConnectionWithLock PlatformLockedConnection();

        private async Task<SQLiteAsyncConnection> Connection()
        {
            if (_connection != null) return _connection;

            Directory.CreateDirectory(RepositoryDirectory);

            _lockedConnection = PlatformLockedConnection();
            _connection = new SQLiteAsyncConnection(() => _lockedConnection);

            await _connection.CreateTableAsync<Company>();
            await _connection.CreateTableAsync<Country>();

            return _connection;
        }

        public string CompanyId { get; set; }

        public string ApplicationName { get; set;}

    }
}
