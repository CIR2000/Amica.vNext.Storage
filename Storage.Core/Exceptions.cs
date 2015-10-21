using System;
using Amica.vNext.Models;

namespace Amica.vNext.Data
{
    public class AdamException : Exception
    {
        public AdamException() { }
	public AdamException(string message) : base(message) { }
        
    }

    public class ObjectNotFoundException : AdamException 
    {
	public ObjectNotFoundException(string id) : base($"Object with id \"{id}\" was not found.") { }
	public ObjectNotFoundException(BaseModel obj) : base($"Object with id \"{obj.UniqueId}\" was not found.") { }
    }


    public class PreconditionFailedException : AdamException
    {
	public PreconditionFailedException(BaseModel obj) : 
	    base($"Object with id \"{obj.UniqueId}\" and etag \"{obj.ETag}\" could not be processed because of an ETag mismatch.") { }
        
        
    }

    // TODO ValidationException should probably support a list of objects and their validation errors. 
    public class ValidationException : AdamException
    {
	public ValidationException(string message) : 
	    base(message) { }
    }


}
