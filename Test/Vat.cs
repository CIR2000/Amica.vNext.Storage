using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Amica.Models;
using Amica.Models.Documents;
using Amica.Storage;
using System;

namespace Test
{
    [TestClass]
    public class Vat : TestBase
    {
        [TestMethod]
        public async Task VatInsertSuccess()
        {
            await CreateAccountAndLoginUser();
            var companyId = (await CreateCompany()).UniqueId;

            var vat = await Remote.Insert(new Amica.Models.Vat { CompanyId = companyId, Name = "Name1", Code = "code1" });
            Assert.IsNotNull(vat.UniqueId);
            vat = await Remote.Insert(new Amica.Models.Vat { CompanyId = companyId, Name = "Name2", Code = "code2", NaturaPA = new Amica.Models.ItalianPA.NaturaPA { Code = "Code", Description = "Description" } });
            Assert.IsNotNull(vat.UniqueId);
        }
        [TestMethod]
        public async Task VatDeleteSuccess()
        {
            await CreateAccountAndLoginUser();
            var companyId = (await CreateCompany()).UniqueId;

            var vat = await Remote.Insert(new Amica.Models.Vat { CompanyId = companyId, Name = "Name", Code = "code" });
            Assert.IsNotNull(vat.UniqueId);

            try { await Remote.Delete(vat); }
            catch (Exception) { throw new AssertFailedException("Exception not expected here."); }

            await Assert.ThrowsExceptionAsync<RemoteObjectNotFoundStorageException>(async () => await Remote.Get(vat));
        }
        [TestMethod]
        public async Task VatValidationFailure()
        {
            await CreateAccountAndLoginUser();
            var companyId = (await CreateCompany()).UniqueId;

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
