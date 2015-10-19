using System;

namespace Amica.vNext.Data
{
    public class ObjectNotFoundException : Exception { }

    // TODO ValidationException should probably support a list of objects and their validation errors. 
    public class ValidationException : Exception { }

}
