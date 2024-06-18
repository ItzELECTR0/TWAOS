using System;
using UnityEngine;
using GameCreator.Runtime.Characters.Animim;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public struct ConfigGesture : IConfig
    {
        [SerializeField] private float m_DelayIn;
        [SerializeField] private float m_Duration;

        [SerializeField] private float m_TransitionIn;
        [SerializeField] private float m_TransitionOut;

        [SerializeField] private float m_Speed;
        [SerializeField] private bool m_RootMotion;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public ConfigGesture(float delayIn, float duration, float speed, bool rootMotion,
            float transitionIn, float transitionOut)
        {
            this.m_DelayIn = delayIn;
            this.m_Duration = duration;
            
            this.m_Speed = speed;
            this.m_RootMotion = rootMotion;
            
            this.m_TransitionIn = transitionIn;
            this.m_TransitionOut = transitionOut;
        }
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public float DelayIn
        {
            get => this.m_DelayIn;
            set => this.m_DelayIn = value;
        }

        public float Duration
        {
            get => this.m_Duration;
            set => this.m_Duration = value;
        }

        public float Speed
        {
            get => this.m_Speed;
            set => this.m_Speed = value;
        }

        public float Weight
        {
            get => 1f;
            set => _ = value;
        }

        public bool RootMotion
        {
            get => this.m_RootMotion;
            set => this.m_RootMotion = value;
        }

        public float TransitionIn
        {
            get => Math.Min(this.m_Duration, this.m_TransitionIn);
            set => this.m_TransitionIn = value;
        }

        public float TransitionOut
        {
            get => Math.Min(this.m_Duration, this.m_TransitionOut);
            set => this.m_TransitionOut = value;
        }
    }
}