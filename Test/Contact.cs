using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Amica.Storage;

namespace Test
{
    [TestClass]
    public class Contact : TestBase
    {
        private Amica.Models.Contact target = new Amica.Models.Contact {
            Name = "Name",
            IdCode = "code",
            Is = new Amica.Models.ContactIs { Active = true, Client = true }
        };

        [TestMethod]
        public async Task ContactInsert()
        {
            await TestInsert(target);
        }
        [TestMethod]
        public async Task ContactGet()
        {
            await TestGet(target);
        }
        [TestMethod]
        public async Task ContactReplace()
        {
            await TestReplace(target, "Name");
        }
        [TestMethod]
        public async Task ContactDelete()
        {
            await TestDelete(target);
        }
        [TestMethod]
        public async Task ContactValidation()
        {
            var companyId = await CreateAccountAndRegisterUserThenStoreCompany();

            // required fields
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Contact()));
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Contact { CompanyId = companyId }));
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Contact { CompanyId = companyId, Name="Name" }));
            await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Contact { CompanyId = companyId, IdCode="Code" }));

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
