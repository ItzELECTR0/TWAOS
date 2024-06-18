using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global List Variable")]
    [Category("Variables/Global List Variable")]
    
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal, typeof(OverlayDot))]
    [Description("Returns the string value of a Global List Variable")]

    [Serializable]
    public class GetStringGlobalList : PropertyTypeGetString
    {
        [SerializeField]
        protected FieldGetGlobalList m_Variable = new FieldGetGlobalList(ValueString.TYPE_ID);

        public override string Get(Args args) => this.m_Variable.Get<string>(args);

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringGlobalList()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}