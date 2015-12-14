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

            if (ApplicationName == null)
                throw new ArgumentNullException(nameof(ApplicationName));

            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                Path.Combine(ApplicationName, "LocalRepository"));

            Directory.CreateDirectory(dbPath);

            var lockedConnection = new SQLiteConnectionWithLock(
                new SQLite.Net.Platform.Generic.SQLitePlatformGeneric(),
                new SQLiteConnectionString(
                    Path.Combine(dbPath, "repository.db3"),
                    true));

            return new SQLiteAsyncConnection(() => lockedConnection);
        }
    }
}
