using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amica.vNext.Models;
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
