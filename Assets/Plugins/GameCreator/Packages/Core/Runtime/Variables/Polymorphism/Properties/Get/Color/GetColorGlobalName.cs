using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global Name Variable")]
    [Category("Variables/Global Name Variable")]
    
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple, typeof(OverlayDot))]
    [Description("Returns the Color value of a Global Name Variable")]

    [Serializable]
    public class GetColorGlobalName : PropertyTypeGetColor
    {
        [SerializeField]
        protected FieldGetGlobalName m_Variable = new FieldGetGlobalName(ValueColor.TYPE_ID);

        public override Color Get(Args args) => this.m_Variable.Get<Color>(args);
        public override string String => this.m_Variable.ToString();
    }
}