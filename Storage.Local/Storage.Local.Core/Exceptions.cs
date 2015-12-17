using System;
using System.Collections.Generic;
using System.Text;
using Amica.vNext.Models;

namespace Amica.vNext.Storage
{
    public class LocalObjectNotFoundRepositoryException : ObjectNotFoundRepositoryException
    {
        public LocalObjectNotFoundRepositoryException(string id) : base(id) { }
        public LocalObjectNotFoundRepositoryException(BaseModel obj) : base(obj) { }
    }
}
