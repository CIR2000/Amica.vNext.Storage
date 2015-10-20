using System;

namespace Amica.vNext.Data
{
    // TODO ex.Message should return the uniqueId of the missing object.
    public class ObjectNotFoundException : Exception { }

    // TODO ValidationException should probably support a list of objects and their validation errors. 
    public class ValidationException : Exception { }

}
