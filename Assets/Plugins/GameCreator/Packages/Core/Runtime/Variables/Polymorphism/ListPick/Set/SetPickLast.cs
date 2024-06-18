using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Last Element")]
    [Category("Last Element")]
    
    [Description("Replaces the element that's at the bottom of the list")]
    [Image(typeof(IconListLast), ColorTheme.Type.Yellow)]

    [Serializable]
    public class SetPickLast : TListSetPick
    {
        public override int GetIndex(ListVariableRuntime list, int count, Args args) => count - 1;
        public override int GetIndex(int count, Args args) => count - 1;
        
        public override string ToString() => "Last";
    }
}