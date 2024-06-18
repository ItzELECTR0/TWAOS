using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("None")]
    [Category("None")]

    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]
    [Description("Returns a null Animation Clip ")]

    [Serializable]
    public class GetAnimationNone : PropertyTypeGetAnimation
    {
        public override AnimationClip Get(Args args) => null;
        public override AnimationClip Get(GameObject gameObject) => null;

        public static PropertyGetAnimation Create => new PropertyGetAnimation(
            new GetAnimationNone()
        );

        public override string String => "None";
    }
}
