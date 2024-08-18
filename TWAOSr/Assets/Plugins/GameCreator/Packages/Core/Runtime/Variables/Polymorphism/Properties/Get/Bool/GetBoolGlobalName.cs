using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global Name Variable")]
    [Category("Variables/Global Name Variable")]
    
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple, typeof(OverlayDot))]
    [Description("Returns the boolean value of a Global Name Variable")]

    [Serializable]
    public class GetBoolGlobalName : PropertyTypeGetBool
    {
        [SerializeField]
        protected FieldGetGlobalName m_Variable = new FieldGetGlobalName(ValueBool.TYPE_ID);

        public override bool Get(Args args) => this.m_Variable.Get<bool>(args);
        public override string String => this.m_Variable.ToString();
    }
}