using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public class MotionToTransform : TMotionTarget<Transform>
    {
        private Vector3 m_LastKnownPosition;

        // INITIALIZERS: --------------------------------------------------------------------------

        public override Character.MovementType Setup(Transform target, float threshold, 
            Action<Character, bool> onFinish)
        {
            this.m_LastKnownPosition = this.m_Target != null
                ? this.m_Target.position
                : this.Transform.position;

            return base.Setup(target, threshold, onFinish);
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        protected override bool HasTarget => this.m_Target != null;

        protected override Vector3 Position
        {
            get
            {
                if (this.m_Target != null) this.m_LastKnownPosition = this.m_Target.position;
                return this.m_LastKnownPosition;
            }
        }

        protected override Vector3 Direction => Vector3.zero;
    }
}