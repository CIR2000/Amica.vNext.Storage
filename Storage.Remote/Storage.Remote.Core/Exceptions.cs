using Amica.vNext.Models;

namespace Amica.vNext.Storage
{
    public class RemoteObjectNotFoundRepositoryException : ObjectNotFoundRepositoryException
    {
        public RemoteObjectNotFoundRepositoryException(string id) : base(id) { }
        public RemoteObjectNotFoundRepositoryException(BaseModel obj) : base(obj) { }
    }

    public class RemoteRepositoryException : RepositoryException
    {
		public RemoteRepositoryException(string message) : base(message) { }
        
    }
    public class PreconditionFailedRepositoryException : RemoteRepositoryException
    {
		public PreconditionFailedRepositoryException(BaseModel obj) : 
			base($"Object with id \"{obj.UniqueId}\" and etag \"{obj.ETag}\" could not be processed because of an ETag mismatch.") { }
    }

    // TODO ValidationRepositoryException should probably support a list of objects and their validation errors. 
    public class ValidationRepositoryException : RemoteRepositoryException
    {
		public ValidationRepositoryException(string message) : 
			base(message) { }
    }
}
