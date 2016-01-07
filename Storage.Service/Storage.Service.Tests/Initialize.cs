using System;
using Amica.vNext.Models;
using Amica.vNext.Storage;
using NUnit.Framework;

namespace Storage.Service.Tests
{
    public class Initialize
	{
	    private static readonly StorageService Service = new StorageService();

	    [TearDown]
	    public void Cleanup()
	    {
	        Service.Dispose();
	    }

	    [Test]
	    public void DefaultProperties()
	    {
	        Assert.That(
				Service.ApplicationName, 
				Is.EqualTo("LocalRepositoryDefaultApplication"));

	        Assert.That(
				Service.ClientId, 
				Is.EqualTo(Environment.GetEnvironmentVariable("SentinelClientId")));

	        Assert.That(
	            Service.Username, 
				Is.Null);

	        Assert.That(
	            Service.Password,
                Is.Null);
	    }

	    [Test]
	    public void InvalidRequests()
	    {
	        var company = new Company
	        {
	            UniqueId = "c1",
                ETag = "etag",

				// TODO investigate why we neet to use UtcNow or objects won't compare
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
	        };

	        Assert.That(
	            async () => await Service.Get(company),
	            Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("Username"));

	        Service.Username = Environment.GetEnvironmentVariable("SentinelUsername");

	        Assert.That(
	            async () => await Service.Get(company),
	            Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("Password"));

	        Service.Password = Environment.GetEnvironmentVariable("SentinelPassword");

	        Assert.That(
	            async () => await Service.Get(company),
	            Throws.TypeOf<RemoteObjectNotFoundStorageException>());
	    }

    }
}
