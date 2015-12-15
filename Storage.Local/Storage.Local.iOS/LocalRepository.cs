using SQLite.Net;
using SQLite.Net.Async;

namespace Amica.vNext.Storage
{
    public class LocalRepository : LocalRepositoryBase
    {
        protected override SQLiteAsyncConnection PlatformConnection()
        {
            var lockedConnection = new SQLiteConnectionWithLock(
                new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS(),
                new SQLiteConnectionString(RepositoryFullPath, true));

            return new SQLiteAsyncConnection(() => lockedConnection);
        }
    }
}
