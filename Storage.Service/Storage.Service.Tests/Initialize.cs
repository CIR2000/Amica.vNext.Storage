using System;
using NUnit.Framework;

namespace Storage.Service.Tests
{
	[TestFixture]
    public class Initialize : TestBase
	{
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
	            Service.Username, Is.Null);

	        Assert.That(
	            Service.Password, Is.Null);
	    }

    }
}
