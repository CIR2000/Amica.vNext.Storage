using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Amica.Storage;

namespace Test
{
    [TestClass]
    public class Vat : TestBase
    {
        private Amica.Models.Vat target = new Amica.Models.Vat { Name = "Name", Code = "code" };

        [TestMethod]
        public async Task VatInsert()
        {
            await TestInsert(target);
        }
        [TestMethod]
        public async Task VatGet()
        {
            await TestGet(target);
        }
        [TestMethod]
        public async Task VatReplace()
        {
            await TestReplace(target, "Name");
        }
        [TestMethod]
        public async Task VatDelete()
        {
            await TestDelete(target);
        }
        [TestMethod]
        public async Task VatValidation()
        {
            var companyId = await CreateAccountAndRegisterUserThenStoreCompany();

            // required fields
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Vat()));
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Vat { CompanyId = companyId }));
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Vat { CompanyId = companyId, Name="Name" }));
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Vat { CompanyId = companyId, Code="Code" }));

            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Vat { CompanyId = companyId, Name="Name", Code="Code", NaturaPA = new Amica.Models.ItalianPA.NaturaPA() }));
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Vat { CompanyId = companyId, Name="Name", Code="Code", NaturaPA = new Amica.Models.ItalianPA.NaturaPA { Code = "Code" }}));
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Vat { CompanyId = companyId, Name="Name", Code="Code", NaturaPA = new Amica.Models.ItalianPA.NaturaPA { Description = "Description" }}));

            // uniqueness
            var vat = await Remote.Insert(new Amica.Models.Vat { CompanyId = companyId, Name = "Name1", Code = "code1" });
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Vat { CompanyId = companyId, Name = "Name1", Code = "Code1" }));

            // referential integrity
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Vat { CompanyId = "123" }));
        }
    }
}
