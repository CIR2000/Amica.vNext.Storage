﻿using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amica.vNext.Models;
using Eve;
using Eve.Authenticators;

namespace Amica.vNext.Data
{
    public class AdamStorage : IBulkStorage
    {
        private readonly Dictionary<Type, string> _resources;
        private readonly Discovery _discovery;
        private readonly EveClient _eve;

        public AdamStorage()
        {

            ClientId = Environment.GetEnvironmentVariable("SentinelClientId");

	    // TODO replace with actual default Uri for the real Adam-DiscoveryService
	    DiscoveryServiceAddress = new Uri("http://10.0.2.2:9000");

	    // Default to local instance for testing purposes, unless an envvar has been set.
            _discovery = new Discovery();
            _eve = new EveClient();

            _resources = new Dictionary<Type, string> {
                {typeof(Company), "companies"},
                {typeof(Country), "countries"}
            };

        }

        private async Task RefreshClientSettings<T>()
        {
            _eve.BaseAddress = await GetAdamAddress();
            _eve.Authenticator = await GetAuthenticator();
            _eve.ResourceName = _resources[typeof (T)];
        }

        private async Task<Uri> GetAdamAddress()
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

        private async Task SetAndValidateResponse(BaseModel obj)
        {
            HttpResponseMessage = _eve.HttpResponse;

            switch (HttpResponseMessage.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    throw new ObjectNotFoundException(obj);
                case (HttpStatusCode) 422:
                    throw new ValidationException(await HttpResponseMessage.Content.ReadAsStringAsync());
                case HttpStatusCode.PreconditionFailed:
                    throw new PreconditionFailedException(obj);
            }
        }

        public async Task<T> Get<T>(string uniqueId) where T : BaseModel, new()
        {
            return await Get(new T {UniqueId = uniqueId});
        }


        public async Task<T> Get<T>(T obj) where T : BaseModel
        {
            await RefreshClientSettings<T>();
            var retObj = await _eve.GetAsync<T>(obj);
            await SetAndValidateResponse(obj);
            return retObj;
        }

        public async Task<T> Insert<T>(T obj) where T : BaseModel
        {
            await RefreshClientSettings<T>();
            var retObj = await _eve.PostAsync<T>(obj);
            await SetAndValidateResponse(retObj);
            return retObj;
        }

        public async Task Delete<T>(T obj) where T : BaseModel
        {
            await RefreshClientSettings<T>();
            HttpResponseMessage = await _eve.DeleteAsync(obj);
            await SetAndValidateResponse(obj);
        }

        public async Task<T> Replace<T>(T obj) where T : BaseModel
        {
            await RefreshClientSettings<T>();
            var retObj = await _eve.PutAsync<T>(obj);
            await SetAndValidateResponse(obj);
            return retObj;
        }

        public async Task<IList<T>> Get<T>()
        {
            return await Get<T>(ifModifiedSince: null);
        }

        public async Task<IList<T>> Get<T>(DateTime? ifModifiedSince)
        {
            await RefreshClientSettings<T>();
            var retObj = await _eve.GetAsync<T>(ifModifiedSince);

            HttpResponseMessage = _eve.HttpResponse;
            if (HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                throw new AdamException($"Resource {_eve.ResourceName} not found on the remote service.");

            return retObj;
        }

        public async Task<IDictionary<string, T>> Get<T>(IEnumerable<string> uniqueIds) where T : BaseModel, new()
        {
	    var retValue = new Dictionary<string, T>();
            foreach (var id in uniqueIds)
            {
                try
                {
                    retValue.Add(id, await Get<T>(id));
                }
		catch (ObjectNotFoundException) { }
            }
            return retValue;
        }

	// TODO this is now the correct implementation as it won't abort the complete batch operation on ValidationException
	// TODO we first need a batch PostAsync method implemented in EveClient, then we use that method here.
        public async Task<IList<T>> Insert<T>(IEnumerable<T> objs) where T : BaseModel
        {
	    var retValue = new List<T>();
            foreach (var obj in objs)
            {
                try
                {
                    retValue.Add(await Insert(obj));
                }
		catch (ValidationException) { }
            }
            return retValue;
        }

        public async Task<IList<string>> Delete<T>(IEnumerable<T> objs) where T : BaseModel
        {
	    var retValue = new List<string>();
            foreach (var id in objs)
            {
                try
                {
                    await Delete(id);
                    retValue.Add(id.UniqueId);
                }
		catch (AdamException) { }
            }
            return retValue;
        }
        public void Dispose()
        {
            _eve?.Dispose();
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
	public HttpResponseMessage HttpResponseMessage { get; private set; }
    }
}
