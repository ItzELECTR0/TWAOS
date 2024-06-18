using System;
using UnityEngine;
using UnityEngine.Animations;

namespace GameCreator.Runtime.Characters.Animim
{
    public class GesturePlayableBehaviour : TAnimimPlayableBehaviour
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        [field: NonSerialized] internal int AnimationClipHash { get; }

        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public GesturePlayableBehaviour(AnimationClip animationClip, AvatarMask avatarMask,
            BlendMode blendMode, AnimimGraph animimGraph, ConfigGesture config) 
            : base(avatarMask, blendMode, animimGraph, config)
        {
            this.AnimationClipHash = animationClip.GetHashCode();
            
            this.AnimatorPlayable = AnimatorControllerPlayable.Create(
                animimGraph.Graph, 
                CreateController(animationClip)
            );
        }
        
        public GesturePlayableBehaviour() : base(null, BlendMode.Blend, null, default)
        { }
    }
}