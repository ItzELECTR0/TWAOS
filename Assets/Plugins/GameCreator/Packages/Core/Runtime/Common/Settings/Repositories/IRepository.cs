namespace GameCreator.Runtime.Common
{
    public interface IRepository
    {
        string AssetDirectory { get; }
        string RepositoryID { get; }
    }
}