using System;
using System.Globalization;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Number Integer")]
    [Category("Values/Number Integer")]
    
    [Image(typeof(IconNumber), ColorTheme.Type.Blue)]
    [Description("The integer part of a numeric value")]

    [Keywords("String", "Value", "Number", "Int", "Long")]
    
    [Serializable]
    public class GetStringValueInteger : PropertyTypeGetString
    {
        [SerializeField] private PropertyGetInteger m_Value = GetDecimalInteger.Create(0);
        [SerializeField] private string m_Format = string.Empty; 

        public override string Get(Args args)
        {
            int value = (int) this.m_Value.Get(args);
            return string.IsNullOrEmpty(this.m_Format)
                ? value.ToString(CultureInfo.InvariantCulture)
                : value.ToString(this.m_Format);
        }

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringValueInteger()
        );

        public override string String => this.m_Value.ToString();
        
        public override string EditorValue => this.m_Value.EditorValue.ToString(CultureInfo.InvariantCulture);
    }
}