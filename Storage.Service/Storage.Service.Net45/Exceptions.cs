using Amica.vNext.Models;

namespace Amica.vNext.Storage
{
    public class ServiceObjectNotFoundStorageException : RemoteObjectNotFoundStorageException
    {
        public ServiceObjectNotFoundStorageException(string id) : base(id) { }
        public ServiceObjectNotFoundStorageException(BaseModel obj) : base(obj) { }
    }
}
