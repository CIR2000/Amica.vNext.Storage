﻿using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Amica.vNext.Models;
using Eve;
using Eve.Authenticators;

namespace Amica.vNext.Storage
{
	// TODO make sure remote access is only attempted if connection is available?

    public class RemoteRepository : IRemoteRepository
    {
        private readonly EveClient _eve = new EveClient();
        private readonly Sentinel _sentinel = new Sentinel();

        private readonly Dictionary<Type, string> _resources = new Dictionary<Type, string>
        {
            { typeof(Company), "companies"},
            { typeof(Country), "countries"}
        };

        private async Task RefreshClientSettings<T>()
		{
			_eve.BaseAddress = await GetAdamAddress();
			_eve.Authenticator = await GetAuthenticator();
			_eve.ResourceName = _resources[typeof (T)];
		}

		private async Task<Uri> GetAdamAddress(bool ignoreCache=false)
		{
			// TODO handle exceptions
			// TODO rename UserData to AmicaData or something equally appropriate.
			var addr = await DiscoveryService.GetServiceAddress(ApiKind.UserData, ignoreCache: ignoreCache);
			return addr;
		}

		public async Task<BearerAuthenticator> GetAuthenticator()
		{
			// TODO is ArgumentNullException appropriate since we're
			// dealing with Properties here (and elsewhere)?
			if (Username == null)
				throw new ArgumentNullException(nameof(Username));
			if (Password == null)
				throw new ArgumentNullException(nameof(Password));
			if (ClientId == null)
				throw new ArgumentNullException(nameof(ClientId));
		    if (DiscoveryService == null)
		        throw new ArgumentNullException(nameof(DiscoveryService));

		    var authAddress = await DiscoveryService.GetServiceAddress(ApiKind.Authentication).ConfigureAwait(false);

			_sentinel.Username = Username;
			_sentinel.Password = Password;
			_sentinel.ClientId = ClientId;
			_sentinel.LocalCache = LocalCache;
			_sentinel.BaseAddress = authAddress;

		    return await _sentinel.GetBearerAuthenticator().ConfigureAwait(false);
		}

        private async Task SetAndValidateResponse(BaseModel obj)
        {
            HttpResponseMessage = _eve.HttpResponse;

            switch (HttpResponseMessage.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    throw new RemoteObjectNotFoundStorageException(obj);
                case (HttpStatusCode) 422:
                    throw new ValidationStorageException(await HttpResponseMessage.Content.ReadAsStringAsync());
                case HttpStatusCode.PreconditionFailed:
                    throw new PreconditionFailedStorageException(obj);
            }
        }

        private delegate Task<T> SingleObjectRequestDelegate<T>(T obj);

        private async Task<bool> ShouldRepeatRequest()
        {
            if (_eve.HttpResponse.StatusCode != HttpStatusCode.NotFound) return false;

            // Cached remote address might be obsolete. Fetch (and cache)
            // a fresh one, and report back that we should try again.
            var address = await GetAdamAddress(true);
            if (address == _eve.BaseAddress) return false;
            _eve.BaseAddress = address;
            return true;
        }
        private async Task<T> PerformRequest<T>(SingleObjectRequestDelegate<T> operation, T obj) where T : BaseModel
        {
            await RefreshClientSettings<T>();

            var retObj = await operation(obj);
			if (await ShouldRepeatRequest())
				retObj = await operation(obj);

            HttpResponseMessage = _eve.HttpResponse;
            await SetAndValidateResponse(obj);
            return retObj;
        }
        /// <summary>
        /// Asyncronoulsy  return a refreshed object from the datastore.
        /// </summary>
        /// <param name="obj">The object to refresh.</param>
        /// <returns>An object from the datastore.</returns>
        /// <exception cref="RemoteObjectNotFoundStorageException"> if <paramref name="obj"/> was not found.</exception>
        public async Task<T> Get<T>(T obj) where T : BaseModel
        {
            return await PerformRequest(_eve.GetAsync<T>, obj);
        }

        public async Task<T> Insert<T>(T obj) where T : BaseModel
        {
            return await PerformRequest(_eve.PostAsync<T>, obj);
        }

        public async Task<T> Replace<T>(T obj) where T : BaseModel
        {
            return await PerformRequest(_eve.PutAsync<T>, obj);
        }

        public async Task Delete<T>(T obj) where T : BaseModel
        {
            await RefreshClientSettings<T>();
            HttpResponseMessage = await _eve.DeleteAsync(obj);
			if (await ShouldRepeatRequest())
					await _eve.DeleteAsync(obj);

            HttpResponseMessage = _eve.HttpResponse;
            await SetAndValidateResponse(obj);
        }

        public async Task<IList<T>> Get<T>() where T : BaseModel
        {
            return await GetInternal<T>(null, null);
        }

        public async Task<IList<T>> Get<T>(string companyId) where T : BaseModelWithCompanyId
        {
            return await GetInternal<T>(null, companyId);
        }

        public async Task<IList<T>> Get<T>(DateTime? ifModifiedSince) where T : BaseModel
        {
            return await GetInternal<T>(ifModifiedSince, null);
        }

        public async Task<IList<T>> Get<T>(DateTime? ifModifiedSince, string companyId) where T : BaseModelWithCompanyId
        {
            return await GetInternal<T>(ifModifiedSince, companyId);
        }

        private async Task<IList<T>> GetInternal<T>(DateTime? ifModifiedSince, string companyId)
        {
            await RefreshClientSettings<T>();

            var rawQuery = companyId != null ? $"{{\"c\": \"{companyId}\"}}" : null;

            var retObj = await _eve.GetAsync<T>(_eve.ResourceName, ifModifiedSince, true, rawQuery);
            if (await ShouldRepeatRequest())
                retObj = await _eve.GetAsync<T>(_eve.ResourceName, ifModifiedSince, true, rawQuery);

            HttpResponseMessage = _eve.HttpResponse;
            if (HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                throw new RemoteStorageException($"Resource {_eve.ResourceName} not found on the remote service.");

            return retObj;
            
        }

        public async Task<IDictionary<string, T>> Get<T>(IEnumerable<string> uniqueIds) where T : BaseModel, new()
        {
            if (uniqueIds == null)
                throw new ArgumentNullException(nameof(uniqueIds));

            var enumerable = uniqueIds as string[] ?? uniqueIds.ToArray();

            var in_ = new StringBuilder();
            foreach (var uniqueId in enumerable)
            {
                in_.Append($"\"{uniqueId}\", ");
            }
            var query = $"{{\"_id\": {{\"$in\": [{in_.ToString().TrimEnd(',', ' ')}]}}}}";

            await RefreshClientSettings<T>();
            var objs = await _eve.GetAsync<T>(_eve.ResourceName, null, rawQuery: query);
			if (await ShouldRepeatRequest())
				objs = await _eve.GetAsync<T>(_eve.ResourceName, null, rawQuery: query);

            HttpResponseMessage = _eve.HttpResponse;
            return objs.ToDictionary(obj => obj.UniqueId);
        }

        public async Task<IList<T>> Insert<T>(IEnumerable<T> objs) where T : BaseModel
        {
            await RefreshClientSettings<T>();

            var enumerable = objs as T[] ?? objs.ToArray();

            var retValue = await _eve.PostAsync(enumerable);
			if (await ShouldRepeatRequest())
				retValue = await _eve.PostAsync(enumerable);

            await SetAndValidateResponse(((List<T>)objs)[0]);
            return retValue;
        }

        /// <summary>
        /// Asyncronously delete a number of objects. If any object could not be found or deleted,
        /// it will be skipped and no exception will be raised.
        /// </summary>
        /// <typeparam name="T">Type of objects to be deleted.</typeparam>
        /// <param name="objs">Objects to be deleted.</param>
        /// <returns>The unique ids of deleted objects.</returns>
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
				catch (RemoteStorageException) { }
            }
            return retValue;
        }

        /// <summary>
        /// Asyncronously delete all objects. Use with caution.
        /// </summary>
        /// <typeparam name="T">Type of objects to be deleted.</typeparam>
        public async Task Delete<T>() where T : BaseModel
        {
            await RefreshClientSettings<T>();
            HttpResponseMessage = await _eve.DeleteAsync();
			if (await ShouldRepeatRequest())
					await _eve.DeleteAsync();

            HttpResponseMessage = _eve.HttpResponse;
        }

        public void Dispose()
        {
            _eve?.Dispose();
        }


        /// <summary>
		/// Used to identify the client against the authentications service. 
		/// </summary>
		public string ClientId { get; set; }

        public Discovery DiscoveryService { get; set; }
        public async Task InvalidateUser(string username)
        {
            await _sentinel.InvalidateUser(username);
        }

        public IBulkObjectCache LocalCache { get; set; }

        /// <summary>
		/// Username. Used to authenticate the user.
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// User password. Needed to authenticate the user.
		/// </summary>
		public string Password { get; set; }

		/// <summary>
        /// Response message returned by the remote service. 
        /// </summary>
		public HttpResponseMessage HttpResponseMessage { get; private set; }
    }
}
