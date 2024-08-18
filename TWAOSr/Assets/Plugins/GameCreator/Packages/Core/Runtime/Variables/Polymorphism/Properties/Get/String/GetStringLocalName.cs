using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local Name Variable")]
    [Category("Variables/Local Name Variable")]

    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]
    [Description("Returns the string value of a Local Name Variable")]
    
    [Serializable]
    public class GetStringLocalName : PropertyTypeGetString
    {
        [SerializeField]
        protected FieldGetLocalName m_Variable = new FieldGetLocalName(ValueString.TYPE_ID);

        public override string Get(Args args) => this.m_Variable.Get<string>(args);

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringLocalName()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}