using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Amica.Models;
using Amica.Storage;

// TODO We are linking BaseModelComparer from Storage.Local.Tests. Maybe we should
// move that class to Storage.Core, or some new Storage.Tests.Core?

namespace Storage.Service.Tests
{
	[TestFixture]
    public class Account : TestBase
	{
	    [Test]
	    public async Task LoginAndLogout()
	    {
			var username = Environment.GetEnvironmentVariable("SentinelUsername");
			var password = Environment.GetEnvironmentVariable("SentinelPassword");

	        var remote = Service.RemoteRepository;

	        Assert.That(
				await remote.Login(new UserAccount() { Username = "hello", Password = "hello" }, false), 
				Is.False);
	        Assert.That(remote.UserAccount.Username, Is.EqualTo(username));
	        Assert.That(remote.UserAccount.Password, Is.EqualTo(password));

	        remote.UserAccount.Username = "hello";
	        remote.UserAccount.Password = "hello";
	        Assert.That(await remote.Login(false), Is.False);
	        Assert.That(await remote.Login(true), Is.False);

	        var challenge = new RemoteRepository() {LocalCache = Service.RemoteRepository.LocalCache};
	        Assert.That(challenge.UserAccount.LoggedIn, Is.False);
	        Assert.That(challenge.UserAccount.Username, Is.Null);

            remote.UserAccount.Username = null;
	        Assert.That(async() => await remote.Login(false),
	            Throws.TypeOf<ArgumentNullException>());

	        remote.UserAccount.Username = username;
            remote.UserAccount.Password = null;
	        Assert.That(async() => await remote.Login(false),
	            Throws.TypeOf<ArgumentNullException>());

	        remote.UserAccount.Username = username;
	        remote.UserAccount.Password = password;
	        remote.UserAccount.ActiveCompany = new Company() {Name = "c1"};

	        Assert.That(await remote.Login(false), Is.True);

	        challenge = new RemoteRepository() {LocalCache = Service.RemoteRepository.LocalCache};
	        Assert.That(challenge.UserAccount.LoggedIn, Is.False);
	        Assert.That(challenge.UserAccount.Username, Is.Null);

	        Assert.That(await remote.Login(true), Is.True);
			
	        challenge = new RemoteRepository() {LocalCache = Service.RemoteRepository.LocalCache};
	        Assert.That(challenge.UserAccount.Username, Is.EqualTo(username));
	        Assert.That(challenge.UserAccount.Password, Is.EqualTo(password));
	        Assert.That(challenge.UserAccount.LoggedIn, Is.True);
	        Assert.That(challenge.UserAccount.ActiveCompany, Is.Not.Null);
	        Assert.That(challenge.UserAccount.ActiveCompany.Name, Is.EqualTo("c1"));

	        await remote.Logout();
	        Assert.That(remote.UserAccount.Username, Is.Null);
	        Assert.That(remote.UserAccount.Password, Is.Null);
	        Assert.That(remote.UserAccount.ActiveCompany, Is.Null);
	        Assert.That(remote.UserAccount.LoggedIn, Is.False);

	        challenge = new RemoteRepository() {LocalCache = Service.RemoteRepository.LocalCache};
	        Assert.That(challenge.UserAccount.Username, Is.Null);
	        Assert.That(challenge.UserAccount.Password, Is.Null);
	        Assert.That(challenge.UserAccount.ActiveCompany, Is.Null);
	        Assert.That(challenge.UserAccount.LoggedIn, Is.False);
	    }
    }
}
