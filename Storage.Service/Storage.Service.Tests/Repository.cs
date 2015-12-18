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

	        var remoteCompany = await StorageService.Remote.Get(updatedCompany);

	        Assert.That(
				remoteCompany, 
					Is.EqualTo(updatedCompany).Using(new Local.Tests.BaseModelComparer()));

	        var localCompany = await StorageService.Local.Get(updatedCompany);

	        Assert.That(
				localCompany, 
					Is.EqualTo(remoteCompany).Using(new Local.Tests.BaseModelComparer()));
	    }

	    [Test]
	    public async Task Replace()
	    {
			Assert.That(
				async () => await Service.Replace(new Company {UniqueId ="c1", ETag="notreally"}), 
				Throws.TypeOf<RemoteObjectNotFoundStorageException>());

	        var insertedCompany = await Service.Insert(new Company {Name ="c1"});

	        var company = new Company { UniqueId = insertedCompany.UniqueId, ETag ="notreally" };

			Assert.That(
				async () => await Service.Replace(company), 
				Throws.TypeOf<PreconditionFailedStorageException>());

	        company.ETag = insertedCompany.ETag;

			Assert.That(
				async () => await Service.Replace(company), 
				Throws.TypeOf<ValidationStorageException>());

	        company.Name = "new c1";

	        var replacedCompany = await Service.Replace(company);

	        var remoteCompany = await StorageService.Remote.Get(replacedCompany);

	        Assert.That(remoteCompany, Is.Not.Null);
	        Assert.That(remoteCompany, Is.EqualTo(replacedCompany).Using(new Local.Tests.BaseModelComparer()));

	        var localCompany = await StorageService.Local.Get(replacedCompany);

	        Assert.That(localCompany, Is.Not.Null);
	        Assert.That(localCompany, Is.EqualTo(remoteCompany).Using(new Local.Tests.BaseModelComparer()));
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

	        Assert.That(
	            async () => await StorageService.Remote.Get(company),
	            Throws.TypeOf<RemoteObjectNotFoundStorageException>());

	        Assert.That(
	            async () => await StorageService.Local.Get(company),
	            Throws.TypeOf<LocalObjectNotFoundStorageException>());
	    }

    }
}
