using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("From List")]
    
    public interface IListSetPick
    {
        int GetIndex(ListVariableRuntime list, int count, Args args);
        int GetIndex(int count, Args args);
        
        string ToString();
    }
}