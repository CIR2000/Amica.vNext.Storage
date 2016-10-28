using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amica.Models;
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

        /// <summary>
        /// Asyncronously get several objects from the datastore.
        /// Eventual missing keys will be ignored and no exception will be raised.
        /// </summary>
        /// <param name="uniqueIds">The ids to look up in the datastore.</param>
        /// <returns>The objects from the datastore.</returns>
        public async Task<IDictionary<string, T>> Get<T>(IEnumerable<string> uniqueIds) where T : BaseModel, new()
        {
            var conn = await Connection();

            var entityName = typeof(T).Name;
            var idList = @"""" + string.Join(@""", """, uniqueIds) + @"""";

			var query = $"SELECT * FROM {entityName} WHERE {"UniqueId"} IN ({idList})";

            var result = await conn.QueryAsync<T>(query);

            return result.ToDictionary(obj => obj.UniqueId);
        }

        /// <summary>
        /// Asyncronously insert several objects into the datastore. 
        /// </summary>
        /// <returns>The inserted object.</returns>
        public async Task<IList<T>> Insert<T>(IEnumerable<T> objs) where T : BaseModel
        {
            var conn = await Connection();
            await conn.InsertAllAsync(objs);
            return (IList<T>)objs;
        }

        /// <summary>
        /// Asyncronously delete a number of objects. If any object could not be found or deleted,
        /// it will be skipped and no exception will be raised.
        /// </summary>
        /// <typeparam name="T">Type of objects to be deleted.</typeparam>
        /// <param name="objs">Objects to be deleted.</param>
        /// <returns>The unique ids of deleted objects.</returns>
        public async Task<IList<string>> Delete<T>(IEnumerable<T> objs) where T : BaseModel
        {
            var conn = await Connection();
            var deleted = new List<string>();

            foreach (var obj in objs)
                if (await conn.DeleteAsync(obj) > 0)
                    deleted.Add(obj.UniqueId);

            return deleted;
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

#region "ILocalBulkRepository"

		/// <summary>
        /// Asyncronoulsy returns  the last datetime at which the collection has been modified.
        /// </summary>
        /// <typeparam name="T">Type of object collection to investigate.</typeparam>
        /// <returns>A DateTime expressing the last time the collection has been modified.</returns>
        public async Task<DateTime> LastModified<T>() where T : BaseModel
        {
            var conn = await Connection();

		    var query = await conn.Table<T>().OrderByDescending(obj => obj.Updated).FirstOrDefaultAsync();
		    return (query == default(T) ? DateTime.MinValue : query.Updated);
        }

        public async Task InsertOrReplace<T>(IEnumerable<T> objs) where T : BaseModel
        {
            var conn = await Connection();
            await conn.InsertOrReplaceAllAsync(objs);
        }

        #endregion

        protected virtual async Task<SQLiteAsyncConnection> Connection()
        {
            if (_connection != null) return _connection;

            _lockedConnection = PlatformLockedConnection();
            _connection = new SQLiteAsyncConnection(() => _lockedConnection);

            // TODO auto-build tables based on Models introspection?
            // Or just complete the manual build process below
            await _connection.CreateTableAsync<Company>();
            await _connection.CreateTableAsync<Vat>();

            return _connection;
        }

        public string RepositoryDirectory
        {
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

        protected abstract string DefaultRepositoryDirectory();

        protected abstract SQLiteConnectionWithLock PlatformLockedConnection();

        public string CompanyId { get; set; }

        public string ApplicationName { get; set;}

    }
}
