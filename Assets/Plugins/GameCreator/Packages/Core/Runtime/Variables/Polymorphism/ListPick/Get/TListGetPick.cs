using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("From List")]
    
    [Serializable]
    public abstract class TListGetPick : IListGetPick
    {
        public abstract int GetIndex(int count, Args args);
    }
}