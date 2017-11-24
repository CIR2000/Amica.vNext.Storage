using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Amica.Storage;
using System;
using DeepEqual.Syntax;

namespace Test
{
    [TestClass]
    public class Fee : TestBase
    {
        private async Task<Amica.Models.Fee> InsertValidObject(string companyId)
        {
            return await Remote.Insert(new Amica.Models.Fee {
                CompanyId = companyId,
                Name = "Fee",
                Amount = 10.1m,
                Vat = new Amica.Models.Vat { Name = "Vat", Code = "Code" }
            });
        }
        [TestMethod]
        public async Task FeeInsert()
        {
            var companyId = await CreateAccountAndRegisterUserThenStoreCompany();

            var fee = await InsertValidObject(companyId);
            Assert.IsNotNull(fee.UniqueId);
        }
        [TestMethod]
        public async Task FeeGet()
        {
            var companyId = await CreateAccountAndRegisterUserThenStoreCompany();
            var fee = await InsertValidObject(companyId);

            var challenge = await Remote.Get(new Amica.Models.Fee { UniqueId = fee.UniqueId });
            fee.ShouldDeepEqual(challenge);
        }
        [TestMethod]
        public async Task FeeReplace()
        {
            var companyId = await CreateAccountAndRegisterUserThenStoreCompany();
            var fee = await InsertValidObject(companyId);

            fee.Name = "New Name";
            fee = await Remote.Replace(fee);

            var challenge = await Remote.Get(new Amica.Models.Fee { UniqueId = fee.UniqueId });
            fee.ShouldDeepEqual(challenge);
        }
        [TestMethod]
        public async Task FeeDelete()
        {
            var companyId = await CreateAccountAndRegisterUserThenStoreCompany();
            var fee = await InsertValidObject(companyId);

            try { await Remote.Delete(fee); }
            catch (Exception) { throw new AssertFailedException("Exception not expected here."); }

            await Assert.ThrowsExceptionAsync<RemoteObjectNotFoundStorageException>(async () => await Remote.Get(fee));
        }
        [TestMethod]
        public async Task FeeValidation()
        {
            var companyId = await CreateAccountAndRegisterUserThenStoreCompany();

            // required fields
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Fee()));
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Fee { CompanyId = companyId }));

            // uniqueness
            var vat = await Remote.Insert(new Amica.Models.Fee { CompanyId = companyId, Name = "Name1", Vat = new Amica.Models.Vat{ Name="Name", Code="Code" }});
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Fee { CompanyId = companyId, Name = "Name1" }));

            // referential integrity
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Fee { CompanyId = "123" }));
        }
    }
}
