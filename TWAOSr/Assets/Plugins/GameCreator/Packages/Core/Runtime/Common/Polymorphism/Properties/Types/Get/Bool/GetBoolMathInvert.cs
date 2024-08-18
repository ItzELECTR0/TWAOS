using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Invert")]
    [Category("Math/Invert")]
    
    [Image(typeof(IconToggleOff), ColorTheme.Type.Red, typeof(OverlayArrowLeft))]
    [Description("Returns False if the Boolean field is True, and True otherwise")]
    
    [Keywords("Fail", "No", "Revert", "Opposite", "Change")]
    [Serializable]
    public class GetBoolMathInvert : PropertyTypeGetBool
    {
        [SerializeField] private PropertyGetBool m_Boolean = GetBoolValue.Create(true);
        
        public override bool Get(Args args) => !this.m_Boolean.Get(args);
        public override bool Get(GameObject gameObject) => !this.m_Boolean.Get(gameObject);

        public static PropertyGetBool Create => new PropertyGetBool(
            new GetBoolMathInvert()
        );

        public override string String => $"not {this.m_Boolean}";
        
        public override bool EditorValue => !this.m_Boolean.EditorValue;
    }
}