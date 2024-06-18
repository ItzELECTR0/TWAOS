using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public abstract class Locomotion8Points
    {
        public AnimationClip m_Idle;
        
        [Space]
        public AnimationClip m_Forward;
        public AnimationClip m_Backward;
        public AnimationClip m_Right;
        public AnimationClip m_Left;
        
        [Space]
        public AnimationClip m_ForwardRight;
        public AnimationClip m_ForwardLeft;
        public AnimationClip m_BackwardRight;
        public AnimationClip m_BackwardLeft;
    }
    
    [Serializable]
    public class Stand8Points : Locomotion8Points
    { }
    
    [Serializable]
    public class Crouch8Points : Locomotion8Points
    { }
}