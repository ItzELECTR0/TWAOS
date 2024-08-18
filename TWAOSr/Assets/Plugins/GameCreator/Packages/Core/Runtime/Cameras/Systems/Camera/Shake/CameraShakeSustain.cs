using System;
using System.Collections.Generic;

namespace GameCreator.Runtime.Cameras
{
    internal class CameraShakeSustain : TCameraShake
    {
        // MEMBERS: -------------------------------------------------------------------------------

        private readonly Dictionary<int, ShakeSystem> m_Shakes;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public CameraShakeSustain()
        {
            this.m_Shakes = new Dictionary<int, ShakeSystem>();
        }
        
        // CALLBACKS: -----------------------------------------------------------------------------

        protected override void OnComplete(ShakeSystem shakeSystem)
        {
            base.OnComplete(shakeSystem);
            this.m_Shakes.Remove(shakeSystem.Layer);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void AddSustain(int layer, float delay, float transition, ShakeEffect shakeEffect)
        {
            if (this.m_Shakes.TryGetValue(layer, out ShakeSystem shakeSystem))
            {
                this.m_Shakes.Remove(layer);
                this.m_ShakeSystems.Remove(shakeSystem);
            }

            shakeSystem = ShakeSystem.Sustain(layer, delay, transition, shakeEffect);
            
            this.m_Shakes.Add(layer, shakeSystem);
            this.m_ShakeSystems.Add(shakeSystem);
        }

        public void RemoveSustain(int layer, float delay, float transition)
        {
            if (this.m_Shakes.TryGetValue(layer, out ShakeSystem shakeSystem))
            {
                shakeSystem.Stop(delay, transition);
            }
        }
    }
}