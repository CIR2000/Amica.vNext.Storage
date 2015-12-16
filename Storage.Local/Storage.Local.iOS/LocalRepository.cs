using SQLite.Net;

namespace Amica.vNext.Storage
{
    public class LocalRepository : LocalRepositoryBase
    {
        protected override SQLiteConnectionWithLock PlatformLockedConnection()
        {
            return new SQLiteConnectionWithLock(
                new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS(),
                new SQLiteConnectionString(RepositoryFullPath, true));
        }
    }
}
