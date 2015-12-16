﻿using System;
using System.Collections;
using System.Threading.Tasks;
using Amica.vNext.Models;
using NUnit.Framework;
using Amica.vNext.Storage;

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
					Throws.TypeOf<ObjectNotFoundRepositoryException>());

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
					Throws.TypeOf<ObjectNotDeletedRepositoryException>());

            Assert.That(
                async () => await Repo.Get(challenge),
				    Throws.TypeOf<ObjectNotFoundRepositoryException>());
	    }

        [Test]
        public async Task Replace()
        {
			var challenge = Challenge();

            Assert.That(
                async () => await Repo.Replace(challenge),
                Throws.TypeOf<ObjectNotReplacedRepositoryException>());

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
