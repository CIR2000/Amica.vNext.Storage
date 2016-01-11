using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Amica.vNext.Models;

namespace Storage.Service.Tests
{
	[TestFixture]
    public class BulkRepository : TestBase
	{
        [Test]
        public async Task Get()
        {
			// Since Get also downloads new objects and deletes obsolete ones, we are
			// going to test all possible scenarios here.

            Assert.That(async () => await Service.Get<Company>(), Is.Empty);
            Assert.That(async () => await Service.Local.Get<Company>(), Is.Empty);

			// Test that adding an object will insert it both remotely and locally.
	        var results = await InsertCompaniesAndValidate();
            var company1 = results[0];
            var company2 = results[1];

			// Test that deleting an object will delete it from both local and remote services.
            await Service.Delete(company1);

            var challenge = await Service.Get<Company>();
            Assert.That(challenge.Count, Is.EqualTo(1));
	        Assert.That(challenge[0], Is.EqualTo(company2).Using(new Local.Tests.BaseModelComparer()));

			// needed in order to make sure that the change below will be stored at a different datetime
            Thread.Sleep(1000);

			// Test that a changed/replaced object is updated both remotely and locally.
            company2.Name = "changed c2";
            var changedCompany2 = await Service.Replace(company2);
            Assert.That(changedCompany2.UniqueId, Is.EqualTo(company2.UniqueId));
            Assert.That(changedCompany2.Created, Is.EqualTo(company2.Created));
            Assert.That(changedCompany2.ETag, Is.Not.EqualTo(company2.ETag));
            Assert.That(changedCompany2.Updated, Is.Not.EqualTo(company2.Updated));

            challenge = await Service.Get<Company>();
            Assert.That(challenge.Count, Is.EqualTo(1));
	        Assert.That(challenge[0], Is.EqualTo(changedCompany2).Using(new Local.Tests.BaseModelComparer()));
            challenge = await Service.Local.Get<Company>();
            Assert.That(challenge.Count, Is.EqualTo(1));
	        Assert.That(challenge[0], Is.EqualTo(changedCompany2).Using(new Local.Tests.BaseModelComparer()));

			// needed in order to make sure that the change below will be stored at a different datetime
            Thread.Sleep(1000);

            // Test that an object changed remotely is downloaded and updated locally.
            company2 = changedCompany2;
            company2.Name = "changed remotely c2";
            changedCompany2 = await Service.Remote.Replace(company2);

            challenge = await Service.Get<Company>();
	        Assert.That(challenge[0], Is.EqualTo(changedCompany2).Using(new Local.Tests.BaseModelComparer()));
            challenge = await Service.Local.Get<Company>();
            Assert.That(challenge.Count, Is.EqualTo(1));
	        Assert.That(challenge[0], Is.EqualTo(changedCompany2).Using(new Local.Tests.BaseModelComparer()));

            challenge = await Service.Get<Company>(changedCompany2.Updated.AddMilliseconds(1));
            Assert.That(challenge.Count, Is.EqualTo(0));
            challenge = await Service.Get<Company>(changedCompany2.Updated);
            Assert.That(challenge.Count, Is.EqualTo(0));
            challenge = await Service.Get<Company>(changedCompany2.Updated.AddMilliseconds(-1));
            Assert.That(challenge.Count, Is.EqualTo(1));
	        Assert.That(challenge[0], Is.EqualTo(changedCompany2).Using(new Local.Tests.BaseModelComparer()));
        }

	    [Test]
	    public async Task Insert()
	    {
			// Since Get also downloads new objects and deletes obsolete ones, we are
			// going to test all possible scenarios here.

            Assert.That(async () => await Service.Get<Company>(), Is.Empty);
            Assert.That(async () => await Service.Local.Get<Company>(), Is.Empty);

	        await InsertCompaniesAndValidate();
	    }

	    private static async Task<IList<Company>> InsertCompaniesAndValidate()
	    {
			var companies = new List<Company>
			{
			    new Company { Name = "c1" },
                new Company { Name = "c2" }
			};

            var results =await Service.Insert<Company>(companies);

            var company1 = results[0];
            var company2 = results[1];

            Assert.That(company1.UniqueId, Is.Not.Null);
            Assert.That(company1.ETag, Is.Not.Null);
            Assert.That(company1.Created, Is.Not.EqualTo(DateTime.MinValue));
            Assert.That(company1.Updated, Is.Not.EqualTo(DateTime.MinValue));
            Assert.That(company2.UniqueId, Is.Not.Null);
            Assert.That(company2.ETag, Is.Not.Null);
            Assert.That(company2.Created, Is.Not.EqualTo(DateTime.MinValue));
            Assert.That(company2.Updated, Is.Not.EqualTo(DateTime.MinValue));

            var challenge = await Service.Get<Company>();
            Assert.That(challenge.Count, Is.EqualTo(2));
	        Assert.That(challenge[0], Is.EqualTo(company1).Using(new Local.Tests.BaseModelComparer()));
	        Assert.That(challenge[1], Is.EqualTo(company2).Using(new Local.Tests.BaseModelComparer()));

            var localChallenge = await Service.Local.Get<Company>();
            Assert.That(localChallenge.Count, Is.EqualTo(2));
	        Assert.That(localChallenge[0], Is.EqualTo(company1).Using(new Local.Tests.BaseModelComparer()));
	        Assert.That(localChallenge[1], Is.EqualTo(company2).Using(new Local.Tests.BaseModelComparer()));
            return results;
	    }
    }
}
