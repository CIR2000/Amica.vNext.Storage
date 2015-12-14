using System;
using System.Collections.Generic;
using System.Text;

namespace Amica.vNext.Storage
{
    public interface ILocalRepository : IRepository
    {
		string DatabasePath { get; set; }
    }
}
