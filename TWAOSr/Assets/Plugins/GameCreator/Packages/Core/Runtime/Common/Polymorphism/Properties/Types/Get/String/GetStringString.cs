using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("String")]
    [Category("Constants/String")]
    
    [Image(typeof(IconString), ColorTheme.Type.Yellow)]
    [Description("A string of characters")]

    [Keywords("String", "Value")]
    
    [Serializable] [HideLabelsInEditor]
    public class GetStringString : PropertyTypeGetString
    {
        [SerializeField] protected string m_Value = "";

        public override string Get(Args args) => this.m_Value;
        public override string Get(GameObject gameObject) => this.m_Value;

        public GetStringString() : base()
        { }

        public GetStringString(string value = "") : this()
        {
            this.m_Value = value;
        }

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringString()
        );

        public override string String => string.IsNullOrEmpty(this.m_Value) ? "<empty>" : this.m_Value;
        
        public override string EditorValue => this.m_Value;
    }
}