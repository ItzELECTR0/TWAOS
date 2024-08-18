using System;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace GameCreator.Runtime.Common
{
    [Title("Animation Clip")]
    
    [Serializable]
    public abstract class PropertyTypeSetAnimation : TPropertyTypeSet<AnimationClip>
    { }
}