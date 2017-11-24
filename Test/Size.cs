using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Amica.Storage;

namespace Test
{
    [TestClass]
    public class Size : TestBase
    {
        private Amica.Models.Size target = new Amica.Models.Size {
                Name = "Size",
            };

        [TestMethod]
        public async Task SizeInsert()
        {
            await TestInsert(target);
        }
        [TestMethod]
        public async Task SizeGet()
        {
            await TestGet(target);
        }
        [TestMethod]
        public async Task SizeReplace()
        {
            await TestReplace(target, "Name");
        }
        [TestMethod]
        public async Task SizeDelete()
        {
            await TestDelete(target);
        }
        [TestMethod]
        public async Task SizeValidation()
        {
            var companyId = await CreateAccountAndRegisterUserThenStoreCompany();

            // required fields
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Size()));
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Size { CompanyId = companyId }));

            // uniqueness
            var vat = await Remote.Insert(new Amica.Models.Size { CompanyId = companyId, Name = "Name1" });
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Size { CompanyId = companyId, Name = "Name1" }));

            // referential integrity
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Size { CompanyId = "123" }));
        }
    }
}
