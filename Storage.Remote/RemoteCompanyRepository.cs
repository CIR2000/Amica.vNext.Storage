using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amica.Models;
using Amica.Models.Documents;

namespace Amica.Storage
{
    public class RemoteCompanyRepository : RemoteBulkRepository, IRemoteCompanyRepository
    {

        public RemoteCompanyRepository()
        {
            const string documents = "documents";

			Resources = new Dictionary<Type, string>
            {
                { typeof(Company), "companies"},
                { typeof(Contact), "contacts"},
                { typeof(Document), documents},
                { typeof(Invoice), documents},
                { typeof(Vat), "vat"},
                { typeof(PaymentOption), "payment-options"},
                { typeof(Fee), "fees"},
            };

        }

        protected override string SetClientEndpoint<T>()
        {
            return Resources[typeof(T)];
        }
        public async Task<IList<T>> Get<T>(string companyId) where T : BaseModelWithCompanyId
        {
            return await GetWithCompanyId<T>(ifModifiedSince: null, companyId: companyId, softDeleted: false);
        }

        public async Task<IList<T>> Get<T>(DateTime? ifModifiedSince, string companyId) where T : BaseModelWithCompanyId
        {
            return await GetWithCompanyId<T>(ifModifiedSince, companyId, softDeleted: false);
        }

        public async Task<IList<T>> Get<T>(DateTime? ifModifiedSince, string companyId, bool softDeleted) where T : BaseModelWithCompanyId
        {
            return await GetWithCompanyId<T>(ifModifiedSince, companyId, softDeleted);
        }
        private async Task<IList<T>> GetWithCompanyId<T>(DateTime? ifModifiedSince, string companyId, bool softDeleted)
        {
            var rawQuery = companyId != null ? $"{{\"company_id\": \"{companyId}\"}}" : null;
            return await GetInternal<T>(ifModifiedSince, rawQuery, softDeleted);
        }
        public Dictionary<Type, string> Resources { get; }
    }
}
