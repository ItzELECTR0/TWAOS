using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public abstract class LocomotionSingle
    {
        public AnimationClip m_Animation;
    }
    
    [Serializable]
    public class StandSingle : LocomotionSingle
    { }
    
    [Serializable]
    public class CrouchSingle : LocomotionSingle
    { }
}