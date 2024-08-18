using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local Name Variable")]
    [Category("Variables/Local Name Variable")]

    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]
    [Description("Returns the Color value of a Local Name Variable")]
    
    [Serializable]
    public class GetColorLocalName : PropertyTypeGetColor
    {
        [SerializeField]
        protected FieldGetLocalName m_Variable = new FieldGetLocalName(ValueColor.TYPE_ID);

        public override Color Get(Args args) => this.m_Variable.Get<Color>(args);
        public override string String => this.m_Variable.ToString();
    }
}