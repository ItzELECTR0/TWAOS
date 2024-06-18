using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public class MotionToLocation : TMotionTarget<Location>
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override bool HasTarget => this.m_Target.HasPosition(this.Character.gameObject);

        protected override Vector3 Position => this.m_Target.HasPosition(this.Character.gameObject) 
            ? this.m_Target.GetPosition(this.Character.gameObject)
            : Vector3.zero;
        
        protected override Vector3 Direction => this.m_Target.HasRotation(this.Character.gameObject)
            ? this.m_Target.GetRotation(this.Character.gameObject) * Vector3.forward
            : Vector3.zero;
    }
}