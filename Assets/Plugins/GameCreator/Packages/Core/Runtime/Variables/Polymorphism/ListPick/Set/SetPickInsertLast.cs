using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Insert Last Element")]
    [Category("Insert Last Element")]
    
    [Description("Inserts a new element at the end on the list")]
    [Image(typeof(IconListLast), ColorTheme.Type.Blue)]

    [Serializable]
    public class SetPickInsertLast : TListSetPick
    {
        public override int GetIndex(ListVariableRuntime list, int count, Args args)
        {
            list.Insert(count, default);
            return count;
        }
        
        public override int GetIndex(int count, Args args) => -1;

        public override string ToString() => "Insert Last";
    }
}