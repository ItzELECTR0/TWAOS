using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    public interface IListVariable
    {
        int Count { get; }
        IdString TypeID { get; }
        
        void Register(Action<ListVariableRuntime.Change, int> callback);
        void Unregister(Action<ListVariableRuntime.Change, int> callback);
    }
}