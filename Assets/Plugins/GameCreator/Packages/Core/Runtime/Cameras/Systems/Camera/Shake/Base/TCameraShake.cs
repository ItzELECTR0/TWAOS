using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    internal abstract class TCameraShake : ICameraShake
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        protected readonly List<ShakeSystem> m_ShakeSystems = new List<ShakeSystem>();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public Vector3 AdditivePosition { get; private set; }
        public Vector3 AdditiveRotation { get; private set; }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Update(TCamera camera)
        {
            this.AdditivePosition = Vector3.zero;
            this.AdditiveRotation = Vector3.zero;

            for (int i = this.m_ShakeSystems.Count - 1; i >= 0; --i)
            {
                this.m_ShakeSystems[i].Update(camera);
                
                this.AdditivePosition += this.m_ShakeSystems[i].ValuePosition;
                this.AdditiveRotation += this.m_ShakeSystems[i].ValueRotation;

                if (this.m_ShakeSystems[i].IsComplete)
                {
                    this.OnComplete(this.m_ShakeSystems[i]);
                    this.m_ShakeSystems.RemoveAt(i);
                }
            }
        }

        // CALLBACKS: -----------------------------------------------------------------------------

        protected virtual void OnComplete(ShakeSystem shakeSystem)
        { }
    }
}