using Amica.vNext.Models;

namespace Amica.vNext.Storage
{
    public class RemoteObjectNotFoundStorageException : ObjectNotFoundStorageException
    {
        public RemoteObjectNotFoundStorageException(string id) : base(id) { }
        public RemoteObjectNotFoundStorageException(BaseModel obj) : base(obj) { }
    }

    public class RemoteStorageException : StorageException
    {
		public RemoteStorageException(string message) : base(message) { }
        
    }
    public class PreconditionFailedStorageException : RemoteStorageException
    {
		public PreconditionFailedStorageException(BaseModel obj) : 
			base($"Object with id \"{obj.UniqueId}\" and etag \"{obj.ETag}\" could not be processed because of an ETag mismatch.") { }
    }

    // TODO ValidationRepositoryException should probably support a list of objects and their validation errors. 
    public class ValidationStorageException : RemoteStorageException
    {
		public ValidationStorageException(string message) : 
			base(message) { }
    }
}
