using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Target Name")]
    [Category("Game Objects/Target Name")]
    
    [Image(typeof(IconTarget), ColorTheme.Type.Yellow)]
    [Description("Returns the name of the targeted game object")]
    
    [Serializable]
    public class GetStringTargetName : PropertyTypeGetString
    {
        public override string Get(Args args) => args.Target != null 
            ? args.Target.name 
            : string.Empty;
        
        public override string Get(GameObject gameObject) => gameObject != null 
            ? gameObject.name 
            : string.Empty;

        public GetStringTargetName() : base()
        { }

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringTargetName()
        );

        public override string String => "Target Name";
    }
}