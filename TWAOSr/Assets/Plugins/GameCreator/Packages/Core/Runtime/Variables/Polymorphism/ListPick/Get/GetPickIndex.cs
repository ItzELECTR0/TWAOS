using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Title("By Index")]
    [Category("By Index")]
    
    [Description("Selects the list element at a specific position")]
    [Image(typeof(IconListIndex), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class GetPickIndex : TListGetPick
    {
        [SerializeField] private PropertyGetInteger m_Index = GetDecimalInteger.Create(0);

        public override int GetIndex(int count, Args args) => (int) this.m_Index.Get(args);

        public override string ToString() => this.m_Index.ToString();
    }
}