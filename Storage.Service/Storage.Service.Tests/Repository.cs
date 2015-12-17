using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Amica.vNext.Models;
using Amica.vNext.Storage;

namespace Storage.Service.Tests
{
	[TestFixture]
    public class Repository : TestBase
	{
	    [Test]
	    public async Task Get()
	    {
	        var company = new Company
	        {
	            UniqueId = "c1",
                ETag = "etag",
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
	        };

	        Assert.That(
	            async () => await Service.Get(company),
	            Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("Username"));

	        Service.Username = "nicola";

	        Assert.That(
	            async () => await Service.Get(company),
	            Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("Password"));

	        Service.Password = "nicola";

	        Assert.That(
	            async () => await Service.Get(company),
	            Throws.TypeOf<RemoteObjectNotFoundRepositoryException>());

	        //await Service.Insert(company);

	        //Assert.That(
	        //    async () => await Service.Get(company),
	        //    Is.EqualTo(company));
	    }

    }
}
