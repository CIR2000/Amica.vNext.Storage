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
	    public async Task GetByCompanyId()
	    {
	        
            Assert.That(async () => await Service.Get<Country>(), Is.Empty);
            Assert.That(async () => await Service.Local.Get<Country>(), Is.Empty);

            var companies = await Service.Insert<Company>(
                new List<Company> {
                    new Company { Name = "c1" },
                    new Company { Name = "c2" }
                });
            var company1 = companies[0];
            var company2 = companies[1];

            var countries =await Service.Insert<Country>(
                new List<Country> {
                    new Country { Name = "c1", CompanyId = company1.UniqueId },
                    new Country { Name = "c2", CompanyId = company2.UniqueId }
                });
            var country1 = countries[0];
            var country2 = countries[1];

			// Get new objects.
            var challenge = await Service.Get<Country>(company1.UniqueId);
            Assert.That(challenge.Count, Is.EqualTo(1));
	        Assert.That(challenge[0], Is.EqualTo(country1).Using(new Local.Tests.BaseModelComparer()));

            var localChallenge = await Service.Local.Get<Country>(company2.UniqueId);
            Assert.That(localChallenge.Count, Is.EqualTo(1));
	        Assert.That(localChallenge[0], Is.EqualTo(country2).Using(new Local.Tests.BaseModelComparer()));

			// Delete, then Get.
            await Service.Delete(country1);
            challenge = await Service.Get<Country>(company1.UniqueId);
            Assert.That(challenge.Count, Is.EqualTo(0));
            challenge = await Service.Local.Get<Country>(company1.UniqueId);
            Assert.That(challenge.Count, Is.EqualTo(0));

            challenge = await Service.Get<Country>(company2.UniqueId);
            Assert.That(challenge.Count, Is.EqualTo(1));
	        Assert.That(challenge[0], Is.EqualTo(country2).Using(new Local.Tests.BaseModelComparer()));

			// Replace, then Get.
	        country2.Name = "changed c2";
            var changed = await Service.Replace(country2);
            challenge = await Service.Get<Country>(company2.UniqueId);
            Assert.That(challenge.Count, Is.EqualTo(1));
	        Assert.That(challenge[0], Is.EqualTo(changed).Using(new Local.Tests.BaseModelComparer()));

            localChallenge = await Service.Local.Get<Country>(company2.UniqueId);
            Assert.That(localChallenge.Count, Is.EqualTo(1));
	        Assert.That(localChallenge[0], Is.EqualTo(changed).Using(new Local.Tests.BaseModelComparer()));

            Thread.Sleep(1000);

            // Remote replace, then Get.
            country2 = changed;
            country2.Name = "changed remotely c2";
            changed = await Service.Remote.Replace(country2);

            challenge = await Service.Get<Country>(company2.UniqueId);
	        Assert.That(challenge[0], Is.EqualTo(changed).Using(new Local.Tests.BaseModelComparer()));
            challenge = await Service.Local.Get<Country>(company2.UniqueId);
            Assert.That(challenge.Count, Is.EqualTo(1));
	        Assert.That(challenge[0], Is.EqualTo(changed).Using(new Local.Tests.BaseModelComparer()));

			// If-Modfied-Since.
            challenge = await Service.Get<Country>(changed.Updated.AddMilliseconds(1), company2.UniqueId);
            Assert.That(challenge.Count, Is.EqualTo(0));
            challenge = await Service.Get<Country>(changed.Updated, company2.UniqueId);
            Assert.That(challenge.Count, Is.EqualTo(0));
            challenge = await Service.Get<Country>(changed.Updated.AddMilliseconds(-1), company2.UniqueId);
            Assert.That(challenge.Count, Is.EqualTo(1));
	        Assert.That(challenge[0], Is.EqualTo(changed).Using(new Local.Tests.BaseModelComparer()));
	    }

        [Test]
        public async Task Get()
        {
			// Since Get also downloads new objects and deletes obsolete ones, we are
			// going to test all possible scenarios here.

            Assert.That(async () => await Service.Get<Company>(), Is.Empty);
            Assert.That(async () => await Service.Local.Get<Company>(), Is.Empty);

	        var results = await InsertCompaniesAndValidate();
            var company1 = results[0];
            var company2 = results[1];

			// Delete, then Get.
            await Service.Delete(company1);
            var challenge = await Service.Get<Company>();
            Assert.That(challenge.Count, Is.EqualTo(1));
	        Assert.That(challenge[0], Is.EqualTo(company2).Using(new Local.Tests.BaseModelComparer()));

            Thread.Sleep(1000);

			// Replace, then Get.
            company2.Name = "changed c2";
            var changedCompany2 = await Service.Replace(company2);

            challenge = await Service.Get<Company>();
            Assert.That(challenge.Count, Is.EqualTo(1));
	        Assert.That(challenge[0], Is.EqualTo(changedCompany2).Using(new Local.Tests.BaseModelComparer()));
            challenge = await Service.Local.Get<Company>();
            Assert.That(challenge.Count, Is.EqualTo(1));
	        Assert.That(challenge[0], Is.EqualTo(changedCompany2).Using(new Local.Tests.BaseModelComparer()));

            Thread.Sleep(1000);

            // Reaplace remotely, then Get.
            company2 = changedCompany2;
            company2.Name = "changed remotely c2";
            changedCompany2 = await Service.Remote.Replace(company2);

            challenge = await Service.Get<Company>();
	        Assert.That(challenge[0], Is.EqualTo(changedCompany2).Using(new Local.Tests.BaseModelComparer()));
            challenge = await Service.Local.Get<Company>();
            Assert.That(challenge.Count, Is.EqualTo(1));
	        Assert.That(challenge[0], Is.EqualTo(changedCompany2).Using(new Local.Tests.BaseModelComparer()));

			// If-Modified-Since.
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
