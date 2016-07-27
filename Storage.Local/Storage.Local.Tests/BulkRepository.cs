using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amica.vNext.Models;
using NUnit.Framework;

namespace Storage.Local.Tests
{
    [TestFixture]
    public class BulkRepository : TestBase
	{
        [Test]
        public async Task Get()
        {
            Assert.That(
                async () => await Repo.Get<Vat>(),
                    Is.Empty);

            Assert.That(
                async () => await Repo.Get<Company>(),
                    Is.Empty);

            var dateChallenge = DateTime.UtcNow;
            var challenge = await Challenge(dateChallenge);

            Assert.That(
                async () => await Repo.Get<Vat>(),
                    Is.Not.Empty
                    .And.EquivalentTo(challenge).Using(new BaseModelComparer()));

            Assert.That(
                async () => await Repo.Get<Company>(),
                    Is.Empty);

        }

        [Test]
        public async Task GetIfModifiedSince()
        {
            var dateChallenge = DateTime.UtcNow;
            var challenge = await Challenge(dateChallenge);

            Assert.That(
                async () => (await Repo.Get<Vat>(dateChallenge.AddMilliseconds(-1))).Count,
                    Is.EqualTo(challenge.Count));

            Assert.That(
                async () => (await Repo.Get<Vat>(dateChallenge)).Count,
                    Is.EqualTo(2));

            Assert.That(
                async () => (await Repo.Get<Vat>(dateChallenge.AddDays(1))).Count,
                    Is.EqualTo(1));

            Assert.That(
                async () => (await Repo.Get<Vat>(dateChallenge.AddDays(2))),
                    Is.Empty);

            Assert.That(
                async () => await Repo.Get<Company>(dateChallenge.AddMilliseconds(-1)),
                    Is.Empty);

        }

        [Test]
        public async Task GetByCompanyId()
        {
            var dateChallenge = DateTime.UtcNow;
            await Challenge(dateChallenge);

            Assert.That(
                async () => await Repo.Get<Vat>("notreally"),
                    Is.Empty);

            Assert.That(
                async () => (await Repo.Get<Vat>("c1")).Count,
                    Is.EqualTo(2));

            Assert.That(
                async () => (await Repo.Get<Vat>("c2")).Count,
                    Is.EqualTo(1));
        }

        [Test]
        public async Task GetIfModifiedSinceByCompanyId()
        {
            var dateChallenge = DateTime.UtcNow;
            await Challenge(dateChallenge);

            Assert.That(
                async () => await Repo.Get<Vat>(dateChallenge, "notreally"),
                    Is.Empty);

            Assert.That(
                async () => (await Repo.Get<Vat>(dateChallenge.AddMilliseconds(-1), "c1")).Count,
                    Is.EqualTo(2));

            Assert.That(
                async () => (await Repo.Get<Vat>(dateChallenge, "c1")).Count,
                    Is.EqualTo(1));

            Assert.That(
                async () => (await Repo.Get<Vat>(dateChallenge.AddDays(1), "c1")).Count,
                    Is.EqualTo(0));

            Assert.That(
                async () => (await Repo.Get<Vat>(dateChallenge, "c2")).Count,
                    Is.EqualTo(1));

            Assert.That(
                async () => (await Repo.Get<Vat>(dateChallenge.AddDays(2), "c2")).Count,
                    Is.EqualTo(0));
        }

	    [Test]
	    public void Insert()
	    {
            var countries = Vats(DateTime.Now).Result;

            Assert.That(
                async () => await Repo.Insert<Vat>(countries),
                Throws.TypeOf<NotImplementedException>());
        }
	    [Test]
	    public void Delete()
	    {
            var countries = Vats(DateTime.Now).Result;

            Assert.That(
                async () => await Repo.Delete<Vat>(countries),
                Throws.TypeOf<NotImplementedException>());
        }
	    [Test]
	    public void GetByUniqueIds()
	    {
	        var ids = new[] {"first", "second"};

            Assert.That(
                async () => await Repo.Get<Vat>(ids),
                Throws.TypeOf<NotImplementedException>());
        }

#pragma warning disable 1998
        private async Task<List<Vat>> Vats(DateTime dateChallenge)
#pragma warning restore 1998
        {
            return new List<Vat>
            {
                new Vat
                {
                    UniqueId = "o1",
                    ETag = "etag1",
                    Created = dateChallenge,
                    Updated = dateChallenge,
                    CompanyId = "c1"
                },
                new Vat
                {
                    UniqueId = "o2",
                    ETag = "etag2",
                    Created = dateChallenge,
                    Updated = dateChallenge.AddDays(1),
                    CompanyId = "c1"
                },
                new Vat
                {
                    UniqueId = "o3",
                    ETag = "etag3",
                    Created = dateChallenge,
                    Updated = dateChallenge.AddDays(2),
                    CompanyId = "c2"
                },
            };
        }

        private async Task<List<Vat>> Challenge(DateTime dateChallenge)
        {
            var challenge = Vats(dateChallenge).Result;

            // TODO replace with bulk insert when it is implemented.
            foreach (var vat in challenge)
                await Repo.Insert(vat);

            return challenge;
        }

    }
}
