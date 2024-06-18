using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Substring")]
    [Category("Math/Substring")]
    
    [Image(typeof(IconString), ColorTheme.Type.Yellow, typeof(OverlayBar))]
    [Description("Extracts a substring based on an index and length")]
    
    [Keywords("String", "Value", "Remove", "Part", "Section")]
    
    [Serializable]
    public class GetStringMathSubstring : PropertyTypeGetString
    {
        [SerializeField] private PropertyGetString m_Text = GetStringString.Create;

        [SerializeField] private PropertyGetInteger m_Index = GetDecimalInteger.Create(0);
        [SerializeField] private PropertyGetInteger m_Length = GetDecimalInteger.Create(5);

        public override string Get(Args args)
        {
            string text = this.m_Text.Get(args);
            
            int index = (int) this.m_Index.Get(args);
            int length = (int) this.m_Length.Get(args);
            
            return text.Substring(index, length);
        }

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringMathSubstring()
        );

        public override string String => $"Substring {this.m_Text}";
    }
}