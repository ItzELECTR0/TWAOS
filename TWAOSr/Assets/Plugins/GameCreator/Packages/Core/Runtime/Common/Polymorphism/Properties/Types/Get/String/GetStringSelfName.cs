using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Self Name")]
    [Category("Game Objects/Self Name")]
    
    [Image(typeof(IconSelf), ColorTheme.Type.Yellow)]
    [Description("Returns the name of the game object which made the call")]
    
    [Serializable]
    public class GetStringSelfName : PropertyTypeGetString
    {
        public override string Get(Args args) => args.Self != null 
            ? args.Self.name 
            : string.Empty;
        
        public override string Get(GameObject gameObject) => gameObject != null 
            ? gameObject.name 
            : string.Empty;

        public GetStringSelfName() : base()
        { }

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringSelfName()
        );

        public override string String => "Self Name";
    }
}