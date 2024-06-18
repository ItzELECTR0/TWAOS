using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local Name Variable")]
    [Category("Variables/Local Name Variable")]

    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]
    [Description("Returns the boolean value of a Local Name Variable")]
    
    [Serializable]
    public class GetBoolLocalName : PropertyTypeGetBool
    {
        [SerializeField]
        protected FieldGetLocalName m_Variable = new FieldGetLocalName(ValueBool.TYPE_ID);

        public override bool Get(Args args) => this.m_Variable.Get<bool>(args);
        public override string String => this.m_Variable.ToString();
    }
}