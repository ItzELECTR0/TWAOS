using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("First Element")]
    [Category("First Element")]
    
    [Description("Replaces the element that appears first on the list")]
    [Image(typeof(IconListFirst), ColorTheme.Type.Yellow)]

    [Serializable]
    public class SetPickFirst : TListSetPick
    {
        public override int GetIndex(ListVariableRuntime list, int count, Args args) => 0;
        public override int GetIndex(int count, Args args) => 0;

        public override string ToString() => "First";
    }
}