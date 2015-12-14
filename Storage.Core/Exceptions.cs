using System;
using Amica.vNext.Models;

namespace Amica.vNext.Storage
{
    public class RepositoryException : Exception
    {
        public RepositoryException() { }
		public RepositoryException(string message) : base(message) { }
        
    }

    public class ObjectNotFoundRepositoryException : RepositoryException 
    {
		public ObjectNotFoundRepositoryException(string id) : base($"Object with id \"{id}\" was not found.") { }
		public ObjectNotFoundRepositoryException(BaseModel obj) : base($"Object with id \"{obj.UniqueId}\" was not found.") { }
    }

    public class ObjectNotReplacedRepositoryException : RepositoryException
    {
        public ObjectNotReplacedRepositoryException (BaseModel obj) :
			base($"Object with id \"{obj.UniqueId}\" could not be replaced.") { }
    }

    public class ObjectNotDeletedRepositoryException : RepositoryException
    {
        public ObjectNotDeletedRepositoryException (BaseModel obj) :
			base($"Object with id \"{obj.UniqueId}\" could not be deleted.") { }
        
    }
}
