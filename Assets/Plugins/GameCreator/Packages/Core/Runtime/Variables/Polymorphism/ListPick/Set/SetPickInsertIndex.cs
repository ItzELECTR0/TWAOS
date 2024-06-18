using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Title("Insert at Index")]
    [Category("Insert at Index")]
    
    [Description("Inserts a new element at the specified list position")]
    [Image(typeof(IconListIndex), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class SetPickInsertIndex : TListSetPick
    {
        [SerializeField] private PropertyGetInteger m_Index = GetDecimalInteger.Create(0);

        public override int GetIndex(ListVariableRuntime list, int count, Args args)
        {
            int index = (int) this.m_Index.Get(args);
            
            list.Insert(index, default);
            return index;
        }
        
        public override int GetIndex(int count, Args args) => -1;

        public override string ToString() => this.m_Index.ToString();
    }
}