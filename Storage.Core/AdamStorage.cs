using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amica.vNext;
using Eve;
using Eve.Authenticators;

namespace Amica.vNext.Data
{
    class AdamStorage : IStorage
    {
        private readonly Dictionary<string, string> _resourcesMapping;

        public AdamStorage()
        {

	    // Default to local instance for testing purposes, unless an envvar has been set.
            var id = Environment.GetEnvironmentVariable("SentinelClientId");
	    DiscoveryServiceUri = id == null ? new Uri("10.0.2.2:5000") : new Uri(id);

            _resourcesMapping = new Dictionary<string, string> {
                {"Aziende", "companies"},
                {"Nazioni", "countries"}
            };

        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<T> Get<T>(string uniqueId)
        {
            var discovery = new Discovery {BaseAddress = DiscoveryServiceUri};

	    // TODO handle exceptions
	    var authUri = discovery.GetServiceUri(ApiKind.Authentication);
            var adamUri = discovery.GetServiceUri(ApiKind.UserData);
            return null;

        }

        private async Task<BearerAuthenticator> GetAuthenticator()
        {
            var sc = new Sentinel
            {
                Username = Username,
                Password = Password,
                ClientId = ClientId,
            };
            return await sc.GetBearerAuthenticator();
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
	public Uri DiscoveryServiceUri { get; set; }
    }
}
