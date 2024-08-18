using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("None")]
    [Category("None")]
    [Description("Don't save on anything")]
    
    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]

    [Serializable]
    public class SetAnimationNone : PropertyTypeSetAnimation
    {
        public override void Set(AnimationClip value, Args args)
        { }
        
        public override void Set(AnimationClip value, GameObject gameObject)
        { }

        public static PropertySetAnimation Create => new PropertySetAnimation(
            new SetAnimationNone()
        );

        public override string String => "(none)";
    }
}