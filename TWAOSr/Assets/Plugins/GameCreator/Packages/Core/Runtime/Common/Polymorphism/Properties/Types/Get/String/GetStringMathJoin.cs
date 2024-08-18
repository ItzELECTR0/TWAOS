using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Join Strings")]
    [Category("Math/Join Strings")]
    
    [Image(typeof(IconString), ColorTheme.Type.Yellow, typeof(OverlayPlus))]
    [Description("Joins two string values")]

    [Keywords("String", "Value", "Concat", "Concatenate", "Stick")]
    
    [Serializable]
    public class GetStringMathJoin : PropertyTypeGetString
    {
        [SerializeField] protected PropertyGetString m_Text1 = GetStringString.Create;
        [SerializeField] protected PropertyGetString m_Text2 = GetStringString.Create;

        public override string Get(Args args)
        {
            string text1 = this.m_Text1.Get(args);
            string text2 = this.m_Text2.Get(args);
            
            return text1 + text2;
        }

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringMathJoin()
        );

        public override string String => $"({this.m_Text1} + {this.m_Text2})";
        
        public override string EditorValue => this.m_Text1.EditorValue + this.m_Text2.EditorValue;
    }
}