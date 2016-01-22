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
	    public void DefaultStorageServiceProperties()
	    {
	        Assert.That(Service.LocalRepository, Is.Null);
	        Assert.That(Service.RemoteRepository, Is.Null);
	    }
	    [Test]
	    public void DefaultRemoteServiceProperties()
	    {
	        var r = new RemoteRepository();
	        Assert.That(r.DiscoveryUri.ToString(), Is.EqualTo("http://10.0.2.2:9000/"));

	        var defaultUri = r.DiscoveryUri;
	        r.DiscoveryUri = new Uri("http://localhost:9000/");
	        r.RestoreDefaults();
	        Assert.That(r.DiscoveryUri.ToString(), Is.EqualTo(defaultUri));
	    }
    }
}
