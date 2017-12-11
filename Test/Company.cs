using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Amica.Storage;
using DeepEqual.Syntax;
using System;

namespace Test
{
    [TestClass]
    public class Company : TestBase
    {
        private Amica.Models.Company.Company target;
        public Company()
        {
            target = new Amica.Models.Company.Company {
                Name = "Name",
                Password = "pw",
                CodiceRea = "123456789",
            };
            target.Predefinizioni.Vat.Code = "code";
            target.Predefinizioni.Vat.Name = "Name";
        }

        [TestMethod]
        public async Task CompanyInsert()
        {
            var account = await CreateAccount();
            var company = await Remote.Insert(target);
            Assert.IsNotNull(company.UniqueId);

        }
        [TestMethod]
        public async Task CompanyGet()
        {
            var account = await CreateAccount();
            target = await Remote.Insert(target);

            var challenge = await Remote.Get(new Amica.Models.Company.Company { UniqueId = target.UniqueId });
            challenge.ShouldDeepEqual(target);
        }
        [TestMethod]
        public async Task CompanyReplace()
        {
            var account = await CreateAccount();
            target = await Remote.Insert(target);

            target.Name = "new name";
            target = await Remote.Replace(target);

            var challenge = await Remote.Get(new Amica.Models.Company.Company { UniqueId = target.UniqueId });
            challenge.ShouldDeepEqual(target);
        }
        [TestMethod]
        public async Task VatDelete()
        {
            var account = await CreateAccount();
            target = await Remote.Insert(target);

            try { await Remote.Delete(target); }
            catch (Exception) { throw new AssertFailedException("Exception not expected here."); }

            await Assert.ThrowsExceptionAsync<RemoteObjectNotFoundStorageException>(async () => await Remote.Get(target));
        }
        //[TestMethod]
        //public async Task VatValidation()
        //{
        //    var companyId = await CreateAccountAndRegisterUserThenStoreCompany();

        //    // required fields
        //    await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Vat()));
        //    await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Vat { CompanyId = companyId }));
        //    await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Vat { CompanyId = companyId, Name="Name" }));
        //    await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Vat { CompanyId = companyId, Code="Code" }));

        //    await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Vat { CompanyId = companyId, Name="Name", Code="Code", NaturaPA = new Amica.Models.ItalianPA.NaturaPA() }));
        //    await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Vat { CompanyId = companyId, Name="Name", Code="Code", NaturaPA = new Amica.Models.ItalianPA.NaturaPA { Code = "Code" }}));
        //    await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Vat { CompanyId = companyId, Name="Name", Code="Code", NaturaPA = new Amica.Models.ItalianPA.NaturaPA { Description = "Description" }}));

        //    // uniqueness
        //    var vat = await Remote.Insert(new Amica.Models.Vat { CompanyId = companyId, Name = "Name1", Code = "code1" });
        //    await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Vat { CompanyId = companyId, Name = "Name1", Code = "Code1" }));

        //    // referential integrity
        //    await Assert.ThrowsExceptionAsync<ValidationStorageException>(async () => await Remote.Insert(new Amica.Models.Vat { CompanyId = "123" }));
        //}
    }
}
