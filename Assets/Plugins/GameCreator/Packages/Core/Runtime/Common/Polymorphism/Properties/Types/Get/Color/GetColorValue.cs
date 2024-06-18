using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Color")]
    [Category("Color")]
    
    [Image(typeof(IconColor), ColorTheme.Type.Pink)]
    [Description("Returns the color value")]

    [Serializable] [HideLabelsInEditor]
    public class GetColorValue : PropertyTypeGetColor
    {
        [SerializeField] 
        protected Color m_Value = Color.white;

        public override Color Get(Args args) => this.m_Value;
        public override Color Get(GameObject gameObject) => this.m_Value;

        public GetColorValue() : base()
        { }

        public GetColorValue(Color value) : this()
        {
            this.m_Value = value;
        }

        public static PropertyGetColor Create(Color value) => new PropertyGetColor(
            new GetColorValue(value)
        );

        public override string String => this.m_Value.a >= 1f
            ? "#" + ColorUtility.ToHtmlStringRGB(this.m_Value)
            : "#" + ColorUtility.ToHtmlStringRGBA(this.m_Value);

        public override Color EditorValue => this.m_Value;
    }
}