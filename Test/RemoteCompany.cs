using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Collections.Generic;
using Amica.Models;
using System.Linq;
using DeepEqual.Syntax;
using System;

namespace Test
{
    [TestClass]
    public class RemoteCompany : TestBase
    {
        private List<Amica.Models.Size> targets = new List<Amica.Models.Size> {
            new Amica.Models.Size{Name="Size1"},
            new Amica.Models.Size{Name="Size2"}
        };

        [TestMethod]
        public async Task GetByCompanyId()
        {
            var objs = await InsertValidObjects(targets);

            var challenge = await RemoteCompany.Get<Amica.Models.Size>(objs.First().CompanyId);

            Assert.AreEqual(objs.ToList().Count, challenge.Count);
            foreach (var obj in objs)
            {
                obj.ShouldDeepEqual(challenge.Where(x=>x.UniqueId == obj.UniqueId).FirstOrDefault());
            }
        }
        [TestMethod]
        public async Task GetByCompanyIdRestrictedByIfModifiedSince()
        {
            var objs = await InsertValidObjects(targets);
            var reference = objs.First();

            var challenge = await RemoteCompany.Get<Amica.Models.Size>(reference.Updated, reference.CompanyId);
            Assert.AreEqual(0, challenge.Count);

            challenge = await RemoteCompany.Get<Amica.Models.Size>(DateTime.Now.AddDays(-1), objs.First().CompanyId);
            Assert.AreEqual(objs.ToList().Count, challenge.Count);
            foreach (var obj in objs)
            {
                obj.ShouldDeepEqual(challenge.Where(x=>x.UniqueId == obj.UniqueId).FirstOrDefault());
            }

        }
        [TestMethod]
        public async Task GetByCompanyIdRestrictedByIfModifiedSinceAndDeleted()
        {
            var objs = await InsertValidObjects(targets);
            var reference = objs.First();

            var challenge = await RemoteCompany.Get<Amica.Models.Size>(reference.Updated, reference.CompanyId, softDeleted:false);
            Assert.AreEqual(0, challenge.Count);

            challenge = await RemoteCompany.Get<Amica.Models.Size>(DateTime.Now.AddDays(-1), objs.First().CompanyId, softDeleted:false);
            Assert.AreEqual(objs.ToList().Count, challenge.Count);
            foreach (var obj in objs)
            {
                obj.ShouldDeepEqual(challenge.Where(x=>x.UniqueId == obj.UniqueId).FirstOrDefault());
            }
            await RemoteCompany.Delete(challenge[0]);
            challenge = await RemoteCompany.Get<Amica.Models.Size>(DateTime.Now.AddDays(-1), reference.CompanyId, softDeleted:true);
            Assert.AreEqual(objs.Count, challenge.Count);

            challenge = await RemoteCompany.Get<Amica.Models.Size>(DateTime.Now.AddDays(-1), reference.CompanyId, softDeleted:false);
            Assert.AreEqual(1, challenge.Count);
        }
        private async Task<IList<T>> InsertValidObjects<T>(IEnumerable<T> objs) where T : BaseModelWithCompanyId
        {
            var companyId = await CreateAccountAndRegisterUserThenStoreCompany();
            foreach (var obj in objs)
            {
                obj.CompanyId = companyId;
            }
            return await RemoteCompany.Insert(objs);
        }
    }
}
