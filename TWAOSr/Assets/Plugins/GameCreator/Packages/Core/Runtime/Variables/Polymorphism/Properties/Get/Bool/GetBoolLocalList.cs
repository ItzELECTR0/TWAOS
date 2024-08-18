using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local List Variable")]
    [Category("Variables/Local List Variable")]
    
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal)]
    [Description("Returns the boolean value of a Local List Variable")]

    [Serializable]
    public class GetBoolLocalList : PropertyTypeGetBool
    {
        [SerializeField]
        protected FieldGetLocalList m_Variable = new FieldGetLocalList(ValueBool.TYPE_ID);

        public override bool Get(Args args) => this.m_Variable.Get<bool>(args);
        public override string String => this.m_Variable.ToString();
    }
}