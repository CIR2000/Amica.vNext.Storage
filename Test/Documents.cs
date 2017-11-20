using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Amica.Models;
using Amica.Models.Documents;

namespace Test
{
    [TestClass]
    public class Documents : TestBase
    {
        [TestMethod]
        public async Task GetDocuments()
        {
            await CreateAndLoginValidUser();

            Remote.Endpoint = "documents";
            var docs = await Remote.Get<Document>();
            Assert.AreEqual(0, docs.Count);
        }
    }
}
