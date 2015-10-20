using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amica.vNext.Models;
using Eve;
using Eve.Authenticators;

namespace Amica.vNext.Data
{
    public class AdamStorage : IStorage
    {
        private readonly Dictionary<Type, string> _resources;
        private readonly Discovery _discovery; 

        public AdamStorage()
        {

            ClientId = Environment.GetEnvironmentVariable("SentinelClientId");

	    // Default to local instance for testing purposes, unless an envvar has been set.
            _discovery = new Discovery();

            _resources = new Dictionary<Type, string> {
                {typeof(Company), "companies"},
                {typeof(Country), "countries"}
            };

        }
        private async Task<Uri> GetServiceAddress()
        {
	    // TODO handle exceptions
	    // TODO rename UserData to AmicaData or something equally appropriate.
            _discovery.BaseAddress = DiscoveryServiceAddress;
            var addr = await _discovery.GetServiceAddress(ApiKind.UserData);
            return addr;
        }
        private async Task<BearerAuthenticator> GetAuthenticator()
        {
	    // TODO is ArgumentNullException appropriate since we're
	    // dealing with Properties here (and elsewhere)?
            if (Username == null)
                throw new ArgumentNullException(nameof(Username));
            if (Password == null)
                throw new ArgumentNullException(nameof(Password));
            if (ClientId == null)
                throw new ArgumentNullException(nameof(ClientId));

            _discovery.BaseAddress = DiscoveryServiceAddress;
	    var authAddress = await _discovery.GetServiceAddress(ApiKind.Authentication);

            var sc = new Sentinel
            {
                Username = Username,
                Password = Password,
                ClientId = ClientId,
		BaseAddress = authAddress
            };
            return await sc.GetBearerAuthenticator();
        }

        public async Task<T> Get<T>(string uniqueId)
        {

            var endpoint = _resources[typeof (T)];

            var rc = new EveClient(await GetServiceAddress(), await GetAuthenticator());
            var item = await rc.GetAsync<T>(endpoint, uniqueId);
            if (item == null)
                throw new ObjectNotFoundException();

            return item;
        }


        public Task<T> Get<T>(T obj)
        {
            throw new NotImplementedException();
        }

        public Task<T> Insert<T>(T obj)
        {
            throw new NotImplementedException();
        }

        public Task<int> Delete<T>(T obj)
        {
            throw new NotImplementedException();
        }

        public Task Replace<T>(T obj)
        {
            throw new NotImplementedException();
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }


	/// <summary>
	/// Used to identify the client against the authentications service. 
	/// </summary>
	public string ClientId { get; set; }

	/// <summary>
	/// Username. Used to authenticate the user.
	/// </summary>
	public string Username { get; set; }

	/// <summary>
	/// User password. Needed to authenticate the user.
	/// </summary>
	public string Password { get; set; }

	/// <summary>
	/// Discovery Service Uri.
	/// </summary>
	public Uri DiscoveryServiceAddress { get; set; }
    }
}
