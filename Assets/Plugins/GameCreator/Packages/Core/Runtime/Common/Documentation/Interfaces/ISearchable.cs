namespace GameCreator.Runtime.Common
{
    public interface ISearchable
    {
        string SearchText  { get; }
        int SearchPriority { get; }
    }
}
