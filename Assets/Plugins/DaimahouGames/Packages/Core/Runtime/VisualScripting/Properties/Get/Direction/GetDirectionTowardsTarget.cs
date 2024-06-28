using System;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.VisualScripting.Direction
{
    [Title("Towards Target")]
    [Category("Towards Target")]
    
    [Image(typeof(IconTarget), ColorTheme.Type.Yellow, typeof(OverlayArrowRight))]
    [Description("A direction pointing towards target")]

    [Serializable]
    public class GetDirectionTowardsTarget : PropertyTypeGetDirection
    {
        public override Vector3 Get(Args args)
        {
            var extendedArgs = ExtendedArgs.Upgrade(ref args);
            if (extendedArgs.Has<Target>())
            {
                var dir = extendedArgs.Get<Target>().Position - args.Self.transform.position;
                return dir;
            }
            
            if(args.Target == null) return Vector3.zero;
            
            return args.Target.transform.position - args.Self.transform.position;
        }

        public override string String => $"Towards Target Direction";

        public static PropertyGetDirection Create => new(new GetDirectionTowardsTarget());
    }
}