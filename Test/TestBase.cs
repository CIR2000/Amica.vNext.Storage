using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Amica.Membership;
using Eve;
using System;
using Eve.Authenticators;
using Amica.Storage;
using Amica.Models;
using System.Net.Http;
using DeepEqual.Syntax;

namespace Test
{
    [TestClass]
    public abstract class TestBase
    {
        protected Membership Membership { get; set; }
        protected RemoteBulkRepository Remote { get; set; }
        protected EveClient Eve { get; }
        public TestBase()
        {
            Eve = new EveClient()
            {
                BaseAddress = new Uri(Environment.GetEnvironmentVariable("MEMBERSHIP_URI") ?? "https://10.0.2.2:8000"),
                ResourceName = Environment.GetEnvironmentVariable("MEMBERSHIP_ENDPOINT") ?? "account",
                Authenticator = new BasicAuthenticator(
                    Environment.GetEnvironmentVariable("MEMBERSHIP_USERNAME") ?? "admin",
                    Environment.GetEnvironmentVariable("MEMBERSHIP_PASSWORD") ?? "secret")
            };
            Eve.CustomHeaders.Add("X-API-Key", Environment.GetEnvironmentVariable("MEMBERSHIP_API_KEY") ?? "admin_key");
        }

        [TestInitialize]
        public virtual async Task Init()
        {
            await (new HttpClient()).PostAsync(new Uri($"{Environment.GetEnvironmentVariable("ADAM_URI") ?? "http://10.0.2.2:5000"}/test/session/start"), null);
            await Eve.DeleteAsync();

            Membership = new Membership();

            Remote = new RemoteBulkRepository
            {
                BaseAddress = new Uri(Environment.GetEnvironmentVariable("ADAM_URI") ?? "http://10.0.2.2:5000"),
                ApiKey = Environment.GetEnvironmentVariable("ADAM_API_KEY") ?? "admin_key",
            };
        }
        protected async Task<string> CreateAccountAndRegisterUserThenStoreCompany()
        {
            var account = new Account { Email = "email@email.com", Vat = "vat", };
            account.User.Add(new User { Username = "user1", Password = "password1", Email = "user1@email.com" });
            account.User.Add(new User { Username = "user2", Password = "password2", Email = "user2@email.com" });
            account =  await Membership.Insert(account);

            Remote.AuthorizationToken = await Membership.Login(account.User[0].Username, account.User[0].Password, account.Email);

            return (await CreateCompany()).UniqueId;
        }
        protected async Task<Company> CreateCompany()
        {
            return await Remote.Insert(new Company { Name = "company" });
        }
        private async Task<T> InsertValidObject<T>(T obj) where T:BaseModelWithCompanyId
        {
            obj.CompanyId = await CreateAccountAndRegisterUserThenStoreCompany();
            return await Remote.Insert(obj);
        }
        public async Task TestGet<T>(T obj) where T: BaseModelWithCompanyId, new()
        {
            obj = await InsertValidObject(obj);

            var challenge = await Remote.Get(new T() { UniqueId = obj.UniqueId });
            obj.ShouldDeepEqual(challenge);
        }
        public async Task TestInsert<T>(T obj) where T: BaseModelWithCompanyId
        {
            obj = await InsertValidObject(obj);
            Assert.IsNotNull(obj.UniqueId);
        }
        public async Task TestReplace<T>(T obj, string property) where T: BaseModelWithCompanyId, new()
        {
            obj = await InsertValidObject(obj);

            var p = obj.GetType().GetProperty(property);
            p.SetValue(obj, "new value");
            obj = await Remote.Replace(obj);

            var challenge = await Remote.Get(new T { UniqueId = obj.UniqueId });
            obj.ShouldDeepEqual(challenge);
        }
        public async Task TestDelete<T>(T obj) where T: BaseModelWithCompanyId
        {
            obj = await InsertValidObject(obj);

            try { await Remote.Delete(obj); }
            catch (Exception) { throw new AssertFailedException("Exception not expected here."); }

            await Assert.ThrowsExceptionAsync<RemoteObjectNotFoundStorageException>(async () => await Remote.Get(obj));

        }
    }
}
