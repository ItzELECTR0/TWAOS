using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Replace Strings")]
    [Category("Math/Replace Strings")]
    
    [Image(typeof(IconString), ColorTheme.Type.Yellow, typeof(OverlayDot))]
    [Description("Replaces all occurrences of a string with another string")]
    
    [Keywords("String", "Value", "Substitute")]
    
    [Serializable]
    public class GetStringMathReplace : PropertyTypeGetString
    {
        [SerializeField] private PropertyGetString m_Text = GetStringString.Create;

        [SerializeField] private PropertyGetString m_OldText = new PropertyGetString("Old Text");
        [SerializeField] private PropertyGetString m_NewText = new PropertyGetString("New Text");

        public override string Get(Args args)
        {
            string text = this.m_Text.Get(args);
            
            string oldText = this.m_OldText.Get(args);
            string newText = this.m_NewText.Get(args);

            return text.Replace(oldText, newText);
        }

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringMathReplace()
        );

        public override string String => $"Replace {this.m_Text}";
    }
}