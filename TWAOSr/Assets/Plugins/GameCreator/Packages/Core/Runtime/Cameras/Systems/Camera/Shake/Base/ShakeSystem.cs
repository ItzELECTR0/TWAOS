using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    internal class ShakeSystem
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        private float m_TransitionIn;
        private float m_TransitionOut;

        private ShakeEffect m_ShakeEffect;

        private float m_CurrentWeight;
        private float m_TargetWeight;
        
        private float m_HoldTransitionInUntil;
        private float m_HoldTransitionOutUntil;

        // PROPERTIES: ----------------------------------------------------------------------------

        public int Layer { get; }
        public bool IsComplete { get; private set; }

        public Vector3 ValuePosition => this.m_ShakeEffect.Value * this.WeightPosition;
        public Vector3 ValueRotation => this.m_ShakeEffect.Value * this.WeightRotation;
        
        private float WeightPosition => this.m_CurrentWeight * this.m_ShakeEffect.PositionWeight;
        private float WeightRotation => this.m_CurrentWeight * this.m_ShakeEffect.RotationWeight;

        // CONSTRUCTORS: --------------------------------------------------------------------------

        private ShakeSystem(int layer, float delay, float transition, float duration, ShakeEffect shakeEffect)
        {
            this.Layer = layer;
            this.IsComplete = false;
            
            this.m_TransitionIn = transition;
            this.m_TransitionOut = duration;
            this.m_ShakeEffect = shakeEffect;

            this.m_HoldTransitionInUntil = Time.time + delay;
            
            this.m_CurrentWeight = transition == 0f ? 1f : 0f;
            this.m_TargetWeight = duration <= 0f ? 1f : 0f;
        }
        
        // STATIC CONSTRUCTORS: -------------------------------------------------------------------

        public static ShakeSystem Sustain(int layer, float delay, float transition, ShakeEffect shakeEffect)
        {
            return new ShakeSystem(layer, delay, transition, -1f, shakeEffect);
        }
        
        public static ShakeSystem Burst(float delay, float duration, ShakeEffect shakeEffect)
        {
            return new ShakeSystem(0, delay, 0f, duration, shakeEffect);
        }
        
        // MODIFIERS: -----------------------------------------------------------------------------

        public void Stop(float delay, float transition)
        {
            this.m_HoldTransitionOutUntil = Time.time + delay;
            this.m_TransitionOut = transition;
            this.m_TargetWeight = 0f;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Update(TCamera camera)
        {
            this.m_ShakeEffect.Update(camera);
            
            float targetWeight = this.m_TargetWeight;
            if (Time.time < this.m_HoldTransitionInUntil) targetWeight = this.m_CurrentWeight;
            if (Time.time < this.m_HoldTransitionOutUntil) targetWeight = this.m_CurrentWeight;

            float distance = targetWeight - this.m_CurrentWeight;
            float deltaSign = Mathf.Approximately(distance, 0f) ? 0f : Mathf.Sign(distance);
            
            float transition = deltaSign > 0f ? this.m_TransitionIn : this.m_TransitionOut;
            float step = Time.deltaTime * deltaSign / transition;
            
            this.m_CurrentWeight = Mathf.Clamp01(this.m_CurrentWeight + step);
            if (this.m_TargetWeight == 0f && this.m_CurrentWeight == 0f) this.IsComplete = true;
        }
    }
}