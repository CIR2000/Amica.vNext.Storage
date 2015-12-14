using System.Threading.Tasks;
using Amica.vNext.Models;
using SQLite.Net.Async;

namespace Amica.vNext.Storage
{
    public abstract class LocalRepositoryBase : ILocalRepository
    {

        private SQLiteAsyncConnection _connection;

        public void Dispose()
        {
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

        public string ApplicationName { get; set; }

        /// <summary>
        /// Returns the appropriate platform connection.
        /// </summary>
        /// <returns>The platform connection.</returns>
        protected abstract SQLiteAsyncConnection PlatformConnection();


        private async Task<SQLiteAsyncConnection> Connection()
        {

            if (_connection != null) return _connection;

            _connection = PlatformConnection();

            //if (DatabasePath == null)
            //    throw new ArgumentNullException(nameof(DatabasePath));

            //_connection = new SQLiteAsyncConnection(DatabasePath, false);

            await _connection.CreateTableAsync<Company>();
            await _connection.CreateTableAsync<Country>();

            return _connection;
        }
    }
}
