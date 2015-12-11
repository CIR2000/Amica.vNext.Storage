namespace Amica.vNext
{
    public interface IRemoteRepository : IBulkRepository
    {
        string Username { get; set; }
        string Password { get; set; }
        string ClientId { get; set; }
    }
}
