using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Empty")]
    [Category("Constants/Empty")]
    
    [Image(typeof(IconEmpty), ColorTheme.Type.Yellow)]
    [Description("An empty string of characters")]

    [Keywords("String", "None", "Null")]
    
    [Serializable]
    public class GetStringEmpty : PropertyTypeGetString
    {
        public const string DISPLAY = "<empty>";
        
        public override string Get(Args args) => string.Empty;
        public override string Get(GameObject gameObject) => string.Empty;

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringEmpty()
        );

        public override string String => DISPLAY;
    }
}