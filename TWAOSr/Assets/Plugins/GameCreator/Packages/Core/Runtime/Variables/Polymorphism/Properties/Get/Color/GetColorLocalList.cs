using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local List Variable")]
    [Category("Variables/Local List Variable")]
    
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal)]
    [Description("Returns the Color value of a Local List Variable")]

    [Serializable]
    public class GetColorLocalList : PropertyTypeGetColor
    {
        [SerializeField]
        protected FieldGetLocalList m_Variable = new FieldGetLocalList(ValueColor.TYPE_ID);

        public override Color Get(Args args) => this.m_Variable.Get<Color>(args);
        public override string String => this.m_Variable.ToString();
    }
}