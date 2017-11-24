using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Amica.Storage;

namespace Test
{
    [TestClass]
    public class Warehouse : TestBase
    {
        private Amica.Models.Warehouse target = new Amica.Models.Warehouse {
                Name = "Warehouse"
            };

        [TestMethod]
        public async Task WarehouseInsert()
        {
            await TestInsert(target);
        }
        [TestMethod]
        public async Task WarehouseGet()
        {
            await TestGet(target);
        }
        [TestMethod]
        public async Task WarehouseReplace()
        {
            await TestReplace(target, "Name");
        }
        [TestMethod]
        public async Task WarehouseDelete()
        {
            await TestDelete(target);
        }
        [TestMethod]
        public async Task SizeValidation()
        {
            var companyId = await CreateAccountAndRegisterUserThenStoreCompany();

            // required fields
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Warehouse()));
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Warehouse { CompanyId = companyId }));

            // uniqueness
            var vat = await Remote.Insert(new Amica.Models.Warehouse { CompanyId = companyId, Name = "Name1" });
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Warehouse { CompanyId = companyId, Name = "Name1" }));

            // referential integrity
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Warehouse { CompanyId = "123" }));
        }
    }
}
