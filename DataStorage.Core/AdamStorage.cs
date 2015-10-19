using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amica.vNext.Data
{
    class AdamStorage : IDataStorage
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
            throw new NotImplementedException();
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
