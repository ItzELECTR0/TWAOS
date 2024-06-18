namespace GameCreator.Editor.Search
{
    internal interface IPipeline
    {
        string Run(string term);
    }
}