using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Amica.vNext.Models;
using Amica.vNext.Storage;

namespace Storage.Service.Tests
{
	[TestFixture]
    public class BulkRepository : TestBase
	{
        [Test]
        public async Task Sync()
        {
            Assert.That(async () => await Service.Get<Company>(), Is.Empty);
            Assert.That(async () => await Service.Local.Get<Company>(), Is.Empty);

			// Test that adding an object will insert it both remotely and locally.
            var company1 = new Company { Name = "c1" };
            company1 = await Service.Insert(company1);
            Assert.That(company1.UniqueId, Is.Not.Null);
            Assert.That(company1.ETag, Is.Not.Null);
            Assert.That(company1.Created, Is.Not.EqualTo(DateTime.MinValue));
            Assert.That(company1.Updated, Is.Not.EqualTo(DateTime.MinValue));

            var challenge = await Service.Get<Company>();
            Assert.That(challenge.Count, Is.EqualTo(1));
	        Assert.That(challenge[0], Is.EqualTo(company1).Using(new Local.Tests.BaseModelComparer()));

            var localChallenge = await Service.Local.Get<Company>();
            Assert.That(localChallenge.Count, Is.EqualTo(1));
	        Assert.That(localChallenge[0], Is.EqualTo(company1).Using(new Local.Tests.BaseModelComparer()));

			// TODO resolve DateTime precision issue!
            Thread.Sleep(2000);

			// Test that an object added to the remote service is downloaded and stored locally.
            var company2 = new Company { Name = "c2" };
            company2 = await Service.Remote.Insert(company2);
            Assert.That(company2, Is.Not.Null);
            Assert.That(company2.ETag, Is.Not.Null);
            Assert.That(company2.Created, Is.Not.EqualTo(DateTime.MinValue));
            Assert.That(company2.Updated, Is.Not.EqualTo(DateTime.MinValue));

            challenge = await Service.Get<Company>();
            Assert.That(challenge.Count, Is.EqualTo(2));
	        Assert.That(challenge[0], Is.EqualTo(company1).Using(new Local.Tests.BaseModelComparer()));
	        Assert.That(challenge[1], Is.EqualTo(company2).Using(new Local.Tests.BaseModelComparer()));

            localChallenge = await Service.Local.Get<Company>();
            Assert.That(localChallenge.Count, Is.EqualTo(2));
	        Assert.That(localChallenge[0], Is.EqualTo(company1).Using(new Local.Tests.BaseModelComparer()));
	        Assert.That(localChallenge[1], Is.EqualTo(company2).Using(new Local.Tests.BaseModelComparer()));

			// Test that deleting an object will delete it from both local and remote services.
            await Service.Delete(company1);
            Assert.That(
                async () => await Service.Local.Get(company1),
                Throws.TypeOf<LocalObjectNotFoundStorageException>());
            Assert.That(
                async () => await Service.Remote.Get(company1),
                Throws.TypeOf<RemoteObjectNotFoundStorageException>());
            challenge = await Service.Get<Company>();
            Assert.That(challenge.Count, Is.EqualTo(1));
	        Assert.That(challenge[0], Is.EqualTo(company2).Using(new Local.Tests.BaseModelComparer()));

			// Test that a changed/replaced object is updated both remotely and locally.
            company2.Name = "changed c2";
            var changedCompany2 = await Service.Replace(company2);
            Assert.That(changedCompany2.UniqueId, Is.EqualTo(company2.UniqueId));
            Assert.That(changedCompany2.Created, Is.EqualTo(company2.Created));
            Assert.That(changedCompany2.ETag, Is.Not.EqualTo(company2.ETag));
            Assert.That(changedCompany2.Updated, Is.Not.EqualTo(company2.Updated));

            var obj = await Service.Get(changedCompany2);
	        Assert.That(obj, Is.EqualTo(changedCompany2).Using(new Local.Tests.BaseModelComparer()));
            obj = await Service.Remote.Get(changedCompany2);
	        Assert.That(obj, Is.EqualTo(changedCompany2).Using(new Local.Tests.BaseModelComparer()));
            obj = await Service.Local.Get(changedCompany2);
	        Assert.That(obj, Is.EqualTo(changedCompany2).Using(new Local.Tests.BaseModelComparer()));
        }
    }
}
