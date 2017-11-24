using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Amica.Storage;

namespace Test
{
    [TestClass]
    public class Fee : TestBase
    {
        private Amica.Models.Fee target = new Amica.Models.Fee {
                Name = "Fee",
                Amount = 10.1m,
                Vat = new Amica.Models.Vat { Name = "Vat", Code = "Code" }
            };

        [TestMethod]
        public async Task FeeInsert()
        {
            await TestInsert(target);
        }
        [TestMethod]
        public async Task FeeGet()
        {
            await TestGet(target);
        }
        [TestMethod]
        public async Task FeeReplace()
        {
            await TestReplace(target, "Name");
        }
        [TestMethod]
        public async Task FeeDelete()
        {
            await TestDelete(target);
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
