using System;
using System.Threading.Tasks;
using Amica.Models;
using NUnit.Framework;
using Amica.Storage;

namespace Storage.Local.Tests
{
    [TestFixture]
    public class Repository : TestBase
	{
        [Test]
        public async Task Get()
        {
            var challenge = Challenge();

            Assert.That(
                async () => await Repo.Get(challenge),
					Throws.TypeOf<LocalObjectNotFoundStorageException>());

            await Repo.Insert(challenge);

            Assert.That(
                async () => await Repo.Get(challenge),
					Is.TypeOf<Company>()
					.And.EqualTo(challenge).Using(new BaseModelComparer()));
        }

        [Test]
        public async Task Insert()
        {
            var challenge = Challenge();

            Assert.That(
                await Repo.Insert(challenge),
					Is.TypeOf<Company>()
					.And.EqualTo(challenge).Using(new BaseModelComparer()));
	    }

        [Test]
        public async Task Delete()
        {
            var challenge = Challenge();

            await Repo.Insert(challenge);
            await Repo.Delete(challenge);

            Assert.That(
                async () => await Repo.Delete(challenge),
					Throws.TypeOf<LocalObjectNotDeletedStorageException>());

            Assert.That(
                async () => await Repo.Get(challenge),
				    Throws.TypeOf<LocalObjectNotFoundStorageException>());
	    }

        [Test]
        public async Task Replace()
        {
			var challenge = Challenge();

            Assert.That(
                async () => await Repo.Replace(challenge),
                Throws.TypeOf<LocalObjectNotReplacedStorageException>());

            await Repo.Insert(challenge);

            challenge.Name = "A Company";
            challenge.Updated = challenge.Updated.AddDays(1);

            Assert.That(
                async () => await Repo.Replace(challenge),
				    Is.TypeOf<Company>()
					.And.EqualTo(challenge).Using(new BaseModelComparer())
                    .And.Property(nameof(Company.Name)).EqualTo(challenge.Name));
        }
	    private static Company Challenge()
	    {
            var now = DateTime.UtcNow;

            return new Company
            {
                UniqueId = "id",
                ETag = "etag",
                Updated = now,
                Created = now
            };
	    }

    }
}
