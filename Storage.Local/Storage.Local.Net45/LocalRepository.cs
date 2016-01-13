using System;
using System.IO;
using SQLite.Net;

namespace Amica.vNext.Storage
{
    public class LocalRepository : LocalRepositoryBase
    {

        protected override string DefaultRepositoryDirectory()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                Path.Combine(ApplicationName, "LocalRepository"));
        }

        protected override SQLiteConnectionWithLock PlatformLockedConnection()
        {
            if (ApplicationName == null)
                throw new ArgumentNullException(nameof(ApplicationName));

            Directory.CreateDirectory(RepositoryDirectory);

            return new SQLiteConnectionWithLock(
                new SQLite.Net.Platform.Generic.SQLitePlatformGeneric(),
                new SQLiteConnectionString(RepositoryFullPath, true));
        }

    }
}
