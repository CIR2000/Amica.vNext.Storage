using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Amica.Storage;

namespace Test
{
    [TestClass]
    public class Payment : TestBase
    {
        private Amica.Models.Payment target = new Amica.Models.Payment {
                Name = "Payment",
                PaymentMethod = new Amica.Models.PaymentMethod { Name="Name"}
            };

        [TestMethod]
        public async Task PaymentInsert()
        {
            await TestInsert(target);
        }
        [TestMethod]
        public async Task PaymentGet()
        {
            await TestGet(target);
        }
        [TestMethod]
        public async Task PaymentReplace()
        {
            await TestReplace(target, "Name");
        }
        [TestMethod]
        public async Task PaymentDelete()
        {
            await TestDelete(target);
        }
        [TestMethod]
        public async Task PaymentValidation()
        {
            var companyId = await CreateAccountAndRegisterUserThenStoreCompany();

            // required fields
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Payment()));
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Payment { CompanyId = companyId }));
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Payment { CompanyId = companyId, Name="Name", FirstPaymentOption=null }));
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Payment { CompanyId = companyId, Name="Name", FirstPaymentDate=null }));
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Payment { CompanyId = companyId, Name="Name", PaymentMethod=null }));

            // uniqueness
            var vat = await Remote.Insert(new Amica.Models.Warehouse { CompanyId = companyId, Name = "Name1" });
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Warehouse { CompanyId = companyId, Name = "Name1" }));

            // referential integrity
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Warehouse { CompanyId = "123" }));
        }
    }
}
