using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Color HDR")]
    [Category("Color HDR")]
    
    [Image(typeof(IconColor), ColorTheme.Type.Pink, typeof(OverlayDot))]
    [Description("Returns the color value with HDR settings")]

    [Serializable] [HideLabelsInEditor]
    public class GetColorValueHDR : PropertyTypeGetColor
    {
        [SerializeField]
        [ColorUsage(true, true)]
        protected Color m_Value = Color.white;

        public override Color Get(Args args) => this.m_Value;
        public override Color Get(GameObject gameObject) => this.m_Value;

        public GetColorValueHDR() : base()
        { }

        public GetColorValueHDR(Color value) : this()
        {
            this.m_Value = value;
        }

        public static PropertyGetColor Create(Color value) => new PropertyGetColor(
            new GetColorValueHDR(value)
        );

        public override string String => this.m_Value.a >= 1f
            ? "#" + ColorUtility.ToHtmlStringRGB(this.m_Value)
            : "#" + ColorUtility.ToHtmlStringRGBA(this.m_Value);

        public override Color EditorValue => this.m_Value;
    }
}