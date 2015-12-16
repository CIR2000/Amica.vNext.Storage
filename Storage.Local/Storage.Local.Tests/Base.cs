using System.IO;
using NUnit.Framework;
using Amica.vNext.Storage;

namespace Storage.Local.Tests
{
    [TestFixture]
    public class TestBase
	{
	    protected LocalRepository Repo;

	    [SetUp]
	    public void Init()
	    {
            Repo = new LocalRepository { ApplicationName = "UnitTest" };
        }

        [TearDown]
	    public void TearDown()
	    {
            Repo.Dispose();

			// cleanup
			if (File.Exists(Repo.RepositoryFullPath))
					File.Delete(Repo.RepositoryFullPath);
        }
    }
}
