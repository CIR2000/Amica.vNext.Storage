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
	            Name = "c1",
	        };

	        Assert.That(
	            async () => await Service.Get(company),
	            Throws.TypeOf<RemoteObjectNotFoundStorageException>());

	        company = await Service.Insert(company);

	        var challenge = new Company {UniqueId = company.UniqueId};

			challenge = await Service.Get(challenge);
	        Assert.That(challenge.ETag, Is.Not.Null);
	        Assert.That(challenge.Created, Is.Not.EqualTo(DateTime.MinValue));
	        Assert.That(challenge.Updated, Is.Not.EqualTo(DateTime.MinValue));
	    }

	    [Test]
	    public async Task Insert()
	    {
	        Assert.That(
	            async () => await Service.Insert(new Company()),
	            Throws.TypeOf<ValidationStorageException>());

	        var company = new Company { Name = "c1" };

	        var updatedCompany = await Service.Insert(company);

	        Assert.That(updatedCompany.UniqueId, Is.Not.Null);
	        Assert.That(updatedCompany.ETag, Is.Not.Null);
	        Assert.That(updatedCompany.Created, Is.Not.EqualTo(DateTime.MinValue));
	        Assert.That(updatedCompany.Updated, Is.Not.EqualTo(DateTime.MinValue));
	    }

	    [Test]
	    public async Task Delete()
	    {
	        var company = new Company
	        {
	            UniqueId = "c1",
	            Name = "c1",
				ETag = "notreally"
	        };

	        Assert.That(
	            async () => await Service.Delete(company),
	            Throws.TypeOf<RemoteObjectNotFoundStorageException>());

            var updatedCompany = await Service.Insert(company);

	        company.UniqueId = updatedCompany.UniqueId;

	        Assert.That(
	            async () => await Service.Delete(company),
	            Throws.TypeOf<PreconditionFailedStorageException>());

	        company.ETag = updatedCompany.ETag;

	        await Service.Delete(company);
	    }

    }
}
