using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertyGetAnimation : TPropertyGet<PropertyTypeGetAnimation, AnimationClip>
    {
        public PropertyGetAnimation() : base(new GetAnimationNone())
        { }

        public PropertyGetAnimation(PropertyTypeGetAnimation defaultType) : base(defaultType)
        { }
    }
}