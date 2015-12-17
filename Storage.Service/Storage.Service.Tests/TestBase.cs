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
	    }

	    [TearDown]
	    public void Cleanup()
	    {
	    }

    }
}
