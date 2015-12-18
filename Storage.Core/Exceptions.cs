using System;
using Amica.vNext.Models;

namespace Amica.vNext.Storage
{
    public class StorageException : Exception
    {
        public StorageException() { }
		public StorageException(string message) : base(message) { }
        
    }

    public class ObjectNotFoundStorageException : StorageException 
    {
		public ObjectNotFoundStorageException(string id) : base($"Object with id \"{id}\" was not found.") { }
		public ObjectNotFoundStorageException(BaseModel obj) : base($"Object with id \"{obj.UniqueId}\" was not found.") { }
    }

    public class ObjectNotReplacedStorageException : StorageException
    {
        public ObjectNotReplacedStorageException (BaseModel obj) :
			base($"Object with id \"{obj.UniqueId}\" could not be replaced.") { }
    }

}
