﻿using System;
using NUnit.Framework;
using Amica.vNext.Storage;

namespace Storage.Service.Tests
{
    [TestFixture]
    public class TestBase
	{
	    protected static readonly StorageService Service = new StorageService();

	    [SetUp]
	    public void Init()
	    {
            Service.ClientId = Environment.GetEnvironmentVariable("SentinelClientId");
            Service.Username = Environment.GetEnvironmentVariable("SentinelUsername");
            Service.Password = Environment.GetEnvironmentVariable("SentinelPassword");
        }

	    [TearDown]
	    public void Cleanup()
	    {
	        Service.Dispose();
	    }

    }
}