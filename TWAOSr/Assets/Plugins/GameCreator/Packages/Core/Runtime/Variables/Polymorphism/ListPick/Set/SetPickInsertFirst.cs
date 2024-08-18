using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Insert First Element")]
    [Category("Insert First Element")]
    
    [Description("Inserts a new element as the first one on the list")]
    [Image(typeof(IconListFirst), ColorTheme.Type.Blue)]

    [Serializable]
    public class SetPickInsertFirst : TListSetPick
    {
        public override int GetIndex(ListVariableRuntime list, int count, Args args)
        {
            list.Insert(0, default);
            return 0;
        }
        
        public override int GetIndex(int count, Args args) => -1;

        public override string ToString() => "Insert First";
    }
}