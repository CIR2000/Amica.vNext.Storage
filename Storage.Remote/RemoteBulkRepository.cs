using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Amica.Models;
using System.Globalization;

namespace Amica.Storage
{
    public class RemoteBulkRepository : RemoteRepository, IRemoteBulkRepository
    {
        public async Task<IList<T>> Get<T>() where T : BaseModel
        {
            return await GetInternal<T>(ifModifiedSince: null, rawQuery: null, softDeleted: false);
        }

        public async Task<IList<T>> Get<T>(DateTime? ifModifiedSince) where T : BaseModel
        {
            return await GetInternal<T>(ifModifiedSince, rawQuery: null, softDeleted: false);
        }

        public async Task<IList<T>> Get<T>(DateTime? ifModifiedSince, bool softDeleted) where T : BaseModel
        {
            return await GetInternal<T>(ifModifiedSince, null, softDeleted);
        }
        protected async Task<IList<T>> GetInternal<T>(DateTime? ifModifiedSince, string rawQuery, bool softDeleted)
        {
            RefreshClientSettings<T>();
            Endpoint = SetClientEndpoint<T>();

            var retObj = await _eve.GetAsync<T>(_eve.ResourceName, ifModifiedSince, softDeleted, rawQuery);

            HttpResponseMessage = _eve.HttpResponse;
            if (HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                throw new RemoteStorageException($"Resource {_eve.ResourceName} not found on the remote service.");

            try
            {
                LastModifiedResponseHeader = DateTime.Parse(
                    HttpResponseMessage.Content.Headers.GetValues("Last-Modified").First(),
                    CultureInfo.InvariantCulture).ToUniversalTime();
            }
            catch (InvalidOperationException)
            {
                // LastModified header is not included with the response.
                // We leave LastModified value untouched.
            }

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

            RefreshClientSettings<T>();
            var objs = await _eve.GetAsync<T>(_eve.ResourceName, null, rawQuery: query);

            HttpResponseMessage = _eve.HttpResponse;
            return objs.ToDictionary(obj => obj.UniqueId);
        }

        public async Task<IList<T>> Insert<T>(IEnumerable<T> objs) where T : BaseModel
        {
            RefreshClientSettings<T>();

            var enumerable = objs as T[] ?? objs.ToArray();

            var retValue = await _eve.PostAsync(enumerable);

            await SetAndValidateResponse(((List<T>)objs)[0]);
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
                catch (RemoteStorageException) { }
            }
            return retValue;
        }

        public async Task Delete<T>() where T : BaseModel
        {
            RefreshClientSettings<T>();
            HttpResponseMessage = await _eve.DeleteAsync();
            HttpResponseMessage = _eve.HttpResponse;
        }
        public DateTime? LastModifiedResponseHeader { get; set; }
    }
}
