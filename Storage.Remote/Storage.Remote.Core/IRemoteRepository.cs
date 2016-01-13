namespace Amica.vNext.Storage
{
    public interface IRemoteRepository : IBulkRepository
    {
		SqliteObjectCacheBase Cache { get; set; }
		Discovery DiscoveryService { get; set; }
    }
}
