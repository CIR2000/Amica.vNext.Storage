using System;
using System.Collections.Generic;
using System.Text;
using Amica.vNext.Models;

namespace Amica.vNext.Storage
{
    public class LocalObjectNotFoundStorageException : ObjectNotFoundStorageException
    {
        public LocalObjectNotFoundStorageException(string id) : base(id) { }
        public LocalObjectNotFoundStorageException(BaseModel obj) : base(obj) { }
    }
    public class LocalObjectNotDeletedStorageException : StorageException
    {
        public LocalObjectNotDeletedStorageException (BaseModel obj) :
			base($"Object with id \"{obj.UniqueId}\" could not be deleted.") { }
        
    }
}
