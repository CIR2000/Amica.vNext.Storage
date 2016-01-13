using System;
using Amica.vNext;
using Amica.vNext.Models;
using NUnit.Framework;
using Amica.vNext.Storage;

namespace Storage.Service.Tests
{
    public abstract class TestBase
    {
        protected static StorageService Service;

	    [SetUp]
	    public void Init()
	    {
	        const string appName = "RemoteServiceTest";

	        var cache = new SqliteObjectCache {ApplicationName = appName};

	        var discovery = new Discovery
	        {
	            BaseAddress = new Uri("http://10.0.2.2:9000/"),
				Cache = cache
	        };

	        var remote = new RemoteRepository
	        {
	            ClientId = Environment.GetEnvironmentVariable("SentinelClientId"),
	            Username = Environment.GetEnvironmentVariable("SentinelUsername"),
	            Password = Environment.GetEnvironmentVariable("SentinelPassword"),
				Cache = cache,
				DiscoveryService = discovery
	        };

	        var local = new LocalRepository
	        {
	            ApplicationName = appName
	        };

	        Service = new StorageService
	        {
				LocalRepository = local,
	            RemoteRepository = remote
	        };

            Service.Delete<Company>().Wait();
            Service.Delete<Country>().Wait();
        }

	    [TearDown]
	    public void Cleanup()
	    {
	        Service.Dispose();
	    }

    }
}
