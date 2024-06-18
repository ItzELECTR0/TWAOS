using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class AirborneDirectional
    {
        public AnimationClip m_UpIdle;
        public AnimationClip m_UpForward;
        public AnimationClip m_UpBackward;
        public AnimationClip m_UpLeft;
        public AnimationClip m_UpRight;
        
        [Space]
        public AnimationClip m_DownIdle;
        public AnimationClip m_DownForward;
        public AnimationClip m_DownBackward;
        public AnimationClip m_DownLeft;
        public AnimationClip m_DownRight;
    }
}