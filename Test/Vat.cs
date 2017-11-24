using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Amica.Storage;
using System;
using DeepEqual.Syntax;

namespace Test
{
    [TestClass]
    public class Vat : TestBase
    {
        private async Task<Amica.Models.Vat> InsertValidVat(string companyId)
        {
            return await Remote.Insert(new Amica.Models.Vat { CompanyId = companyId, Name = "Name", Code = "code" });
        }
        [TestMethod]
        public async Task VatInsert()
        {
            var companyId = await CreateAccountAndRegisterUserThenStoreCompany();

            var vat = await InsertValidVat(companyId);
            Assert.IsNotNull(vat.UniqueId);

            vat = await Remote.Insert(new Amica.Models.Vat { CompanyId = companyId, Name = "Name2", Code = "code2", NaturaPA = new Amica.Models.ItalianPA.NaturaPA { Code = "Code", Description = "Description" } });
            Assert.IsNotNull(vat.UniqueId);
        }
        [TestMethod]
        public async Task VatGet()
        {
            var companyId = await CreateAccountAndRegisterUserThenStoreCompany();
            var vat = await InsertValidVat(companyId);

            var challenge = await Remote.Get(new Amica.Models.Vat { UniqueId = vat.UniqueId });
            vat.ShouldDeepEqual(challenge);
        }
        [TestMethod]
        public async Task VatReplace()
        {
            var companyId = await CreateAccountAndRegisterUserThenStoreCompany();
            var vat = await InsertValidVat(companyId);

            vat.Name = "New Name";
            vat = await Remote.Replace(vat);

            var challenge = await Remote.Get(new Amica.Models.Vat { UniqueId = vat.UniqueId });
            vat.ShouldDeepEqual(challenge);
        }
        [TestMethod]
        public async Task VatDelete()
        {
            var companyId = await CreateAccountAndRegisterUserThenStoreCompany();
            var vat = await InsertValidVat(companyId);

            try { await Remote.Delete(vat); }
            catch (Exception) { throw new AssertFailedException("Exception not expected here."); }

            await Assert.ThrowsExceptionAsync<RemoteObjectNotFoundStorageException>(async () => await Remote.Get(vat));
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
