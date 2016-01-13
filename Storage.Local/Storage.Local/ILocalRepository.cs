namespace Amica.vNext.Storage
{
    public interface ILocalRepository : IRepository
    {
        string ApplicationName { get; set; }
    }
}
