using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertySetAnimation : TPropertySet<PropertyTypeSetAnimation, AnimationClip>
    {
        public PropertySetAnimation() : base(new SetAnimationNone())
        { }

        public PropertySetAnimation(PropertyTypeSetAnimation defaultType) : base(defaultType)
        { }
    }
}