using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class ExitAnimationClip
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private AnimationClip m_ExitClip;
        [SerializeField] private AvatarMask m_ExitMask;
        
        [SerializeField] private bool m_RootMotion;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public AnimationClip ExitClip => m_ExitClip;
        public AvatarMask ExitMask => m_ExitMask;
        
        public bool RootMotion => this.m_RootMotion;
    }
}