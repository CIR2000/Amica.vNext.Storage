using System;
using Amica.vNext.Models;

namespace Amica.vNext.Data
{
    public class StorageException : Exception
    {
        public StorageException() { }
		public StorageException(string message) : base(message) { }
        
    }

    public class AdamStorageException : StorageException
    {
        public AdamStorageException() { }
		public AdamStorageException(string message) : base(message) { }
    }

    public class ObjectNotFoundStorageException : AdamStorageException 
    {
		public ObjectNotFoundStorageException(string id) : base($"Object with id \"{id}\" was not found.") { }
		public ObjectNotFoundStorageException(BaseModel obj) : base($"Object with id \"{obj.UniqueId}\" was not found.") { }
    }

    public class PreconditionFailedStorageException : AdamStorageException
    {
		public PreconditionFailedStorageException(BaseModel obj) : 
			base($"Object with id \"{obj.UniqueId}\" and etag \"{obj.ETag}\" could not be processed because of an ETag mismatch.") { }
    }

    // TODO ValidationStorageException should probably support a list of objects and their validation errors. 
    public class ValidationStorageException : AdamStorageException
    {
		public ValidationStorageException(string message) : 
			base(message) { }
    }


}
