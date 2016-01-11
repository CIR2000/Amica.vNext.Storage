using System;
using Amica.vNext.Models;
using NUnit.Framework;
using Amica.vNext.Storage;

namespace Storage.Service.Tests
{
    public abstract class TestBase
	{
	    protected static readonly StorageService Service = new StorageService();

	    [SetUp]
	    public void Init()
	    {
            Service.ClientId = Environment.GetEnvironmentVariable("SentinelClientId");
            Service.Username = Environment.GetEnvironmentVariable("SentinelUsername");
            Service.Password = Environment.GetEnvironmentVariable("SentinelPassword");

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
