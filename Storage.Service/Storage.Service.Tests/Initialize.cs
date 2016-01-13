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
	        Assert.That(Service.Cache, Is.Null);
	        Assert.That(Service.DiscoveryService, Is.Null);
	        Assert.That(Service.LocalRepository, Is.Null);
	        Assert.That(Service.RemoteRepository, Is.Null);
	    }
    }
}
