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
    public class Initialization : TestBase
	{
        [Test]
        public void ApplicationName()
        {
            var repo = new LocalRepository {ApplicationName = null};

	        Assert.That(
				async () => await repo.Get<Company>(), 
				Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo(nameof(ApplicationName)));
	    }

	    [Test]
	    public void DefaultRepositoryLocation()
	    {
	        var repo = new LocalRepository();
	        File.Delete(repo.RepositoryFullPath);

	        Assert.That(async () => await repo.Get<Company>(), Is.Empty);
	        Assert.That(File.Exists(repo.RepositoryFullPath), Is.True);
	    }
	    [Test]
	    public void CustomRepositoryLocation()
	    {

	        const string file = "tests.db3";
	        var dir = Environment.CurrentDirectory;
	        var challenge = Path.Combine(dir, file);

	        var repo = new LocalRepository {RepositoryDirectory = dir, RepositoryFileName = file };
	        Assert.That(challenge, Is.EqualTo(repo.RepositoryFullPath));

	        File.Delete(repo.RepositoryFullPath);

			// test that we don't raise ArgumentNullException on ApplicationName in this case
	        Assert.That(async () => await repo.Get<Company>(), Is.Empty);
	        Assert.That(File.Exists(challenge), Is.True);
	    }
    }
}
