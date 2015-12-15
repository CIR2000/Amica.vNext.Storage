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
    public class ObjectMethods : TestBase
	{
        [Test]
        public async Task Get()
        {
            var now = DateTime.UtcNow;

            var challenge = new Company
            {
                UniqueId = "id",
                ETag = "etag",
                Updated = now,
                Created = now
            };

            Assert.That(
                async () => await Repo.Get(challenge),
					Throws.TypeOf<ObjectNotFoundRepositoryException>());

            await Repo.Insert(challenge);

            Assert.That(
                async () => await Repo.Get(challenge),
					Is.TypeOf<Company>()
					.And.Property(nameof(Company.UniqueId)).EqualTo(challenge.UniqueId)
					.And.Property(nameof(Company.ETag)).EqualTo(challenge.ETag)
					.And.Property(nameof(Company.Updated)).EqualTo(challenge.Updated)
					.And.Property(nameof(Company.Created)).EqualTo(challenge.Created));
	    }

        [Test]
        public async Task Insert()
        {
            var now = DateTime.UtcNow;

            var challenge = new Company
            {
                UniqueId = "id",
                ETag = "etag",
                Updated = now,
                Created = now
            };

            Assert.That(
                await Repo.Insert(challenge),
					Is.TypeOf<Company>()
					.And.Property(nameof(Company.UniqueId)).EqualTo(challenge.UniqueId)
					.And.Property(nameof(Company.ETag)).EqualTo(challenge.ETag)
					.And.Property(nameof(Company.Updated)).EqualTo(challenge.Updated)
					.And.Property(nameof(Company.Created)).EqualTo(challenge.Created));
	    }

        [Test]
        public async Task Delete()
        {
            var now = DateTime.UtcNow;

            var challenge = new Company
            {
                UniqueId = "id",
                ETag = "etag",
                Updated = now,
                Created = now
            };

            await Repo.Insert(challenge);
            await Repo.Delete(challenge);

            Assert.That(
                async () => await Repo.Delete(challenge),
                Throws.TypeOf<ObjectNotDeletedRepositoryException>());

            Assert.That(
                async () => await Repo.Get(challenge),
                Throws.TypeOf<ObjectNotFoundRepositoryException>());
	    }
    }
}
