namespace Amica.vNext.Storage
{
    public interface IRemoteRepository : IBulkRepository, IApplicationName
    {
        string Username { get; set; }
        string Password { get; set; }
        string ClientId { get; set; }
    }
}
