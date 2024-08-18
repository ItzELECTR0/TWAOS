using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Title("At Index")]
    [Category("At Index")]
    
    [Description("Replaces the list element at a specific position")]
    [Image(typeof(IconListIndex), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class SetPickIndex : TListSetPick
    {
        [SerializeField] private PropertyGetInteger m_Index = GetDecimalInteger.Create(0);

        public override int GetIndex(ListVariableRuntime list, int count, Args args) => (int) this.m_Index.Get(args);
        public override int GetIndex(int count, Args args) => (int) this.m_Index.Get(args);

        public override string ToString() => this.m_Index.ToString();
    }
}