using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amica.Models;

namespace Amica.Storage
{
    public interface IRemoteBulkRepository: IRemoteRepository, IBulkRepository
    {
    }
}
