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
	    private LocalRepository _repo;

	    [SetUp]
	    public void Init()
	    {
	        _repo = new LocalRepository {ApplicationName = "UnitTest"};
	        File.Delete(_repo.RepositoryFullPath);
	    }

	    [TearDown]
	    public void TearDown()
	    {
            _repo.Dispose();
        }
    }
}
