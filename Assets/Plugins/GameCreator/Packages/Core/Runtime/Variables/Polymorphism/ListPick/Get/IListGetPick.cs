using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("From List")]
    
    public interface IListGetPick
    {
        int GetIndex(int count, Args args);
        string ToString();
    }
}