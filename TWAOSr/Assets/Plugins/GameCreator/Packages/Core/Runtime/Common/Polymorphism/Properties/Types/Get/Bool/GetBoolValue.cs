using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Value")]
    [Category("Value")]
    
    [Image(typeof(IconToggleOn), ColorTheme.Type.Red)]
    [Description("Returns true if the checkbox is ticked. False otherwise")]
    
    [Keywords("Toggle", "Checkbox", "Enable", "Disable", "Active", "Inactive")]
    [Serializable] [HideLabelsInEditor]
    public class GetBoolValue : PropertyTypeGetBool
    {
        [SerializeField] protected bool m_Value = true;

        public override bool Get(Args args) => this.m_Value;
        public override bool Get(GameObject gameObject) => this.m_Value;

        public GetBoolValue() : base()
        { }

        public GetBoolValue(bool value = true) : this()
        {
            this.m_Value = value;
        }

        public static PropertyGetBool Create(bool value) => new PropertyGetBool(
            new GetBoolValue(value)
        );

        public override string String => this.m_Value ? "True" : "False";
        
        public override bool EditorValue => this.m_Value;
    }
}