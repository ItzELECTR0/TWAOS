using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Identity")]
    [Category("Math/Identity")]
    
    [Image(typeof(IconRotation), ColorTheme.Type.TextNormal)]
    [Description("A rotation that represents no rotation at all")]

    [Serializable]
    public class GetRotationIdentity : PropertyTypeGetRotation
    {
        public override Quaternion Get(Args args) => Quaternion.identity;
        public override Quaternion Get(GameObject gameObject) => Quaternion.identity;

        public static PropertyGetRotation Create => new PropertyGetRotation(
            new GetRotationIdentity()
        );

        public override string String => "Identity";
    }
}