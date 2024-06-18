using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public abstract class Locomotion16Points
    {
        public AnimationClip m_Idle;
        
        [Space]
        public AnimationClip m_ForwardFast;
        public AnimationClip m_BackwardFast;
        public AnimationClip m_RightFast;
        public AnimationClip m_LeftFast;

        [Space]
        public AnimationClip m_ForwardRightFast;
        public AnimationClip m_ForwardLeftFast;
        public AnimationClip m_BackwardRightFast;
        public AnimationClip m_BackwardLeftFast;
        
        [Space]
        public AnimationClip m_ForwardSlow;
        public AnimationClip m_BackwardSlow;
        public AnimationClip m_RightSlow;
        public AnimationClip m_LeftSlow;
        
        [Space]
        public AnimationClip m_ForwardRightSlow;
        public AnimationClip m_ForwardLeftSlow;
        public AnimationClip m_BackwardRightSlow;
        public AnimationClip m_BackwardLeftSlow;
    }
    
    [Serializable]
    public class Stand16Points : Locomotion16Points
    { }
    
    [Serializable]
    public class Crouch16Points : Locomotion16Points
    { }
}