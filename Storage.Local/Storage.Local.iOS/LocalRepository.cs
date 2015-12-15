using System;
using System.IO;
using SQLite.Net;
using SQLite.Net.Async;

namespace Amica.vNext.Storage
{
    public class LocalRepository : LocalRepositoryBase
    {
        protected override SQLiteAsyncConnection PlatformConnection()
        {
            var dbPath = DatabaseFolder();

            Directory.CreateDirectory(dbPath);

            var lockedConnection = new SQLiteConnectionWithLock(
                new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS(),
                new SQLiteConnectionString(
                    Path.Combine(dbPath, "repository.db3"),
                    true));

            return new SQLiteAsyncConnection(() => lockedConnection);
        }
    }
}
