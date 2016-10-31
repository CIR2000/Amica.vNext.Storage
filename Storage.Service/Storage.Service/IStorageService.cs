namespace Amica.Storage
{
    public interface IStorageService : IBulkRepositoryRemote
    {
		ILocalBulkRepository LocalRepository { get; set; }
		IRemoteRepository RemoteRepository { get; set; }
		bool SilentlyFailOnRemoteReadExceptions { get; set; }
    }
}