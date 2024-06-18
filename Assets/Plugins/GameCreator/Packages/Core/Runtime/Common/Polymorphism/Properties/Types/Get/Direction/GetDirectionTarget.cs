using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Target Direction")]
    [Category("Game Objects/Target Direction")]
    
    [Image(typeof(IconTarget), ColorTheme.Type.Yellow)]
    [Description("The forward direction of the target game object in World Space")]

    [Serializable]
    public class GetDirectionTarget : PropertyTypeGetDirection
    {
        public GetDirectionTarget()
        { }
        
        public override Vector3 Get(Args args)
        {
            GameObject target = args.Target;
            return target != null ? target.transform.forward : default;
        }

        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionTarget()
        );

        public override string String => "Target Direction";
    }
}