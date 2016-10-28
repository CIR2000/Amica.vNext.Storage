using System;
using Amica.Models;
using NUnit.Framework;
using Amica.vNext.Storage;
using SimpleObjectCache;

namespace Storage.Service.Tests
{
    public abstract class TestBase
    {
        protected static StorageService Service;

	    [SetUp]
	    public void Init()
	    {
	        const string appName = "RemoteServiceTest";

	        var remote = new RemoteRepository
	        {
	            DiscoveryUri = new Uri("http://10.0.2.2:9000/"),
	            LocalCache = new SqliteObjectCache {ApplicationName = appName},
	            ClientId = Environment.GetEnvironmentVariable("SentinelClientId"),
	            UserAccount = new UserAccount
	            {
	                Username = Environment.GetEnvironmentVariable("SentinelUsername"),
	                Password = Environment.GetEnvironmentVariable("SentinelPassword"),
	            }
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
            Service.Delete<Vat>().Wait();
        }

	    [TearDown]
	    public void Cleanup()
	    {
	        Service.RemoteRepository.Logout().Wait();
	        Service.Dispose();
	    }

    }
}
