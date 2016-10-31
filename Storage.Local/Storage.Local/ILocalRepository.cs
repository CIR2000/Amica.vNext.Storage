namespace Amica.Storage
{
    public interface ILocalRepository : IRepository
    {
        string ApplicationName { get; set; }
    }
}
