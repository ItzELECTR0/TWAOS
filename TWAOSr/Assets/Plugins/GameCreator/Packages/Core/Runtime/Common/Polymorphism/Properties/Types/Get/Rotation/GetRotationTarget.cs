using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Target Rotation")]
    [Category("Game Objects/Target Rotation")]
    
    [Image(typeof(IconTarget), ColorTheme.Type.Yellow)]
    [Description("Rotation of the targeted game object in local or world space")]

    [Serializable] [HideLabelsInEditor]
    public class GetRotationTarget : PropertyTypeGetRotation
    {
        [SerializeField] private RotationSpace m_Space = RotationSpace.Global;

        public override Quaternion Get(Args args)
        {
            return args.Target != null 
                ? this.m_Space == RotationSpace.Global 
                    ? args.Target.transform.rotation
                    : args.Target.transform.localRotation
                : default;
        }

        public static PropertyGetRotation Create => new PropertyGetRotation(
            new GetRotationTarget()
        );

        public override string String => $"{this.m_Space} Target";
    }
}