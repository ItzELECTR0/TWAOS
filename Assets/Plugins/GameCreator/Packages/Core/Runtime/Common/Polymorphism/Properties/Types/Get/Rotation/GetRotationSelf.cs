using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Self Rotation")]
    [Category("Game Objects/Self Rotation")]
    
    [Image(typeof(IconSelf), ColorTheme.Type.Yellow)]
    [Description("Rotation of the game object making the call in local or world space")]

    [Serializable] [HideLabelsInEditor]
    public class GetRotationSelf : PropertyTypeGetRotation
    {
        [SerializeField] private RotationSpace m_Space = RotationSpace.Global;

        public override Quaternion Get(Args args)
        {
            return args.Self != null 
                ? this.m_Space == RotationSpace.Global 
                    ? args.Self.transform.rotation
                    : args.Self.transform.localRotation
                : default;
        }

        public static PropertyGetRotation Create => new PropertyGetRotation(
            new GetRotationSelf()
        );

        public override string String => $"{this.m_Space} Self";
    }
}