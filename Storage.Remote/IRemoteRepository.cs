using System;
using System.Net.Http;

namespace Amica.Storage
{
    public interface IRemoteRepository: IRepository
    {
        string AuthorizationToken { get; set; }
		UserAccount UserAccount { get; set; }
        Uri BaseAddress { get; set; }
        string Endpoint { get; set; }
        string ApiKey { get; set; }
        HttpResponseMessage HttpResponseMessage { get; }
    }
}
