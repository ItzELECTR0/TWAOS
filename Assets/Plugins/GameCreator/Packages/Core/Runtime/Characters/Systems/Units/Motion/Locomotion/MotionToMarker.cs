using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    public class MotionToMarker : TMotionTarget<Marker>
    {
        private Vector3 m_LastKnownPosition;

        // PROPERTIES: ----------------------------------------------------------------------------

        protected override bool HasTarget => this.m_Target != null;

        protected override Vector3 Position
        {
            get
            {
                if (this.m_Target != null)
                {
                    Vector3 position = this.m_Target.GetPosition(this.Character.gameObject);
                    this.m_LastKnownPosition = position;
                }
                
                return this.m_LastKnownPosition;
            }
        }

        protected override Vector3 Direction
        {
            get
            {
                if (this.m_Target == null) return default;
                return this.m_Target.GetDirection(this.Character.gameObject);
            }
        }
    }
}