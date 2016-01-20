using Storage.Remote;

namespace Amica.vNext.Storage
{
    public interface IStorageService : IBulkRepositoryRemote
    {
		ILocalBulkRepository LocalRepository { get; set; }
		IRemoteRepository RemoteRepository { get; set; }
		bool SilentlyFailOnRemoteReadExceptions { get; set; }
    }
}