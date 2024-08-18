using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Self Direction")]
    [Category("Game Objects/Self Direction")]
    
    [Image(typeof(IconSelf), ColorTheme.Type.Yellow)]
    [Description("The forward direction of the caller game object in World Space")]

    [Serializable]
    public class GetDirectionSelf : PropertyTypeGetDirection
    {
        public GetDirectionSelf()
        { }
        
        public override Vector3 Get(Args args)
        {
            GameObject self = args.Self;
            return self != null ? self.transform.forward : default;
        }

        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionSelf()
        );

        public override string String => "Self Direction";
    }
}