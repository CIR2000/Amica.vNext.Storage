using System;
using Amica.vNext.Models;

namespace Amica.vNext.Storage
{
    public class RepositoryException : Exception
    {
        public RepositoryException() { }
		public RepositoryException(string message) : base(message) { }
        
    }

    public class RemoteRepositoryException : RepositoryException
    {
        public RemoteRepositoryException() { }
		public RemoteRepositoryException(string message) : base(message) { }
    }

    public class ObjectNotFoundRepositoryException : RemoteRepositoryException 
    {
		public ObjectNotFoundRepositoryException(string id) : base($"Object with id \"{id}\" was not found.") { }
		public ObjectNotFoundRepositoryException(BaseModel obj) : base($"Object with id \"{obj.UniqueId}\" was not found.") { }
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
