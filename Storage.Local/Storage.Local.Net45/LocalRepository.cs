using SQLite.Net;
using SQLite.Net.Async;

namespace Amica.vNext.Storage
{
    public class LocalRepository : LocalRepositoryBase
    {
        protected override SQLiteAsyncConnection PlatformConnection()
        {
			LockedConnection = new SQLiteConnectionWithLock(
                new SQLite.Net.Platform.Generic.SQLitePlatformGeneric(),
                new SQLiteConnectionString(RepositoryFullPath, true));

            return new SQLiteAsyncConnection(() => LockedConnection);
        }
    }
}
