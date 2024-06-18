using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global Name Variable")]
    [Category("Variables/Global Name Variable")]
    
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple, typeof(OverlayDot))]
    [Description("Returns the string value of a Global Name Variable")]

    [Serializable]
    public class GetStringGlobalName : PropertyTypeGetString
    {
        [SerializeField]
        protected FieldGetGlobalName m_Variable = new FieldGetGlobalName(ValueString.TYPE_ID);

        public override string Get(Args args) => this.m_Variable.Get<string>(args);

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringGlobalName()
        );

        public override string String => this.m_Variable.ToString();
    }
}