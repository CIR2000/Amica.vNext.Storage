using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amica.Models;
using Eve;
using Eve.Authenticators;
using Amica.Models.Documents;
using System.Collections.Generic;

namespace Amica.Storage
{
    public class RemoteRepository : IRemoteRepository
    {
        protected readonly EveClient _eve = new EveClient();
        private delegate Task<T> SingleObjectRequestDelegate<T>(T obj);
        private Dictionary<Type, string> _endpoints = new Dictionary<Type, string>
        {
            {typeof(Document), "documents" },
        };

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
            RefreshClientSettings<T>();
            HttpResponseMessage = await _eve.DeleteAsync(obj);

            HttpResponseMessage = _eve.HttpResponse;
            await SetAndValidateResponse(obj);
        }

        public void Dispose()
        {
            _eve.Dispose();
        }
        private async Task<T> PerformRequest<T>(SingleObjectRequestDelegate<T> operation, T obj) where T : BaseModel
        {
            RefreshClientSettings<T>();
            var retObj = await operation(obj);
            HttpResponseMessage = _eve.HttpResponse;
            await SetAndValidateResponse(obj);

            return retObj;
        }
        protected virtual string SetClientEndpoint<T>()
        {
            try {
                return _endpoints[typeof(T)];
            }
            catch (KeyNotFoundException)
            {
                return Endpoint;
            }
            
        }
        protected void RefreshClientSettings<T>()
        {
            if (BaseAddress == null) { throw new ArgumentNullException(nameof(BaseAddress)); }
            if (ApiKey == null) { throw new ArgumentNullException(nameof(ApiKey)); }

            _eve.BaseAddress = BaseAddress; 
            _eve.CustomHeaders["X-API-Key"] = ApiKey;
            _eve.ResourceName = SetClientEndpoint<T>();

            if (AuthorizationToken != null) {
                _eve.Authenticator = new BearerAuthenticator(AuthorizationToken);
                return;
            }
            if (UserAccount != null) {
                _eve.Authenticator = new BasicAuthenticator(UserAccount.Username, UserAccount.Password);
                return;
            }
            _eve.Authenticator = null;
        }

        protected async Task SetAndValidateResponse(BaseModel obj)
        {
            HttpResponseMessage = _eve.HttpResponse;

            switch (HttpResponseMessage.StatusCode)
            {
                case HttpStatusCode.Created:
                case HttpStatusCode.NoContent:
                case HttpStatusCode.OK:
                case HttpStatusCode.NotModified:
                    return;
                case HttpStatusCode.NotFound:
                    throw new RemoteObjectNotFoundStorageException(obj);
                case (HttpStatusCode)422:
                    throw new ValidationStorageException(await HttpResponseMessage.Content.ReadAsStringAsync());
                case HttpStatusCode.PreconditionFailed:
                    throw new PreconditionFailedStorageException(obj);
                case HttpStatusCode.Unauthorized:
                    throw new AuthorizationFailedStorageException();
                default:
                    throw new RemoteStorageException(await HttpResponseMessage.Content.ReadAsStringAsync());

            }
        }

        public HttpResponseMessage HttpResponseMessage { get; protected set; }
        public UserAccount UserAccount { get; set; }
        public Uri BaseAddress { get; set; }
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public string AuthorizationToken { get; set; }
    }
}
