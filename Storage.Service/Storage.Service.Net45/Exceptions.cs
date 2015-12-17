using Amica.vNext.Models;

namespace Amica.vNext.Storage
{
    public class StorageServiceObjectNotFoundRepositoryException : RemoteObjectNotFoundRepositoryException
    {
        public StorageServiceObjectNotFoundRepositoryException(string id) : base(id) { }
        public StorageServiceObjectNotFoundRepositoryException(BaseModel obj) : base(obj) { }
    }
}
