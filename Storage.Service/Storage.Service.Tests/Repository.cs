using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Amica.vNext.Models;
using Amica.vNext.Storage;
using System.Net.Http;

namespace Storage.Service.Tests
{
	[TestFixture]
    public class Repository : TestBase
	{
	    [Test]
	    public async Task Insert()
	    {

	        var company = new Company
	        {
	            Name = "c1"
	        };

	        var updatedCompany = await Service.Insert(company);

	        Assert.That(updatedCompany.UniqueId, Is.Not.Empty);
	    }

    }
}
