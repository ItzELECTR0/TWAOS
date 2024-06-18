using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public readonly struct PositionTowards : IPosition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private readonly Transform m_Target;
        [NonSerialized] private readonly Vector3 m_Axis;
        
        [NonSerialized] private readonly Vector3 m_Offset;
        [NonSerialized] private readonly float m_Distance;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public PositionTowards(Transform target, Vector3 axis, Vector3 offset, float distance)
        {
            this.m_Target = target;
            this.m_Axis = new Vector3(
                Mathf.Approximately(axis.x, 0f) ? 0f : 1f,
                Mathf.Approximately(axis.y, 0f) ? 0f : 1f,
                Mathf.Approximately(axis.z, 0f) ? 0f : 1f
            );
            
            this.m_Offset = offset;
            this.m_Distance = distance;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public bool HasPosition(GameObject user) => user != null && this.m_Target != null;
        
        public Vector3 GetPosition(GameObject source)
        {
            Vector3 position = this.m_Target.TransformPoint(this.m_Offset);
            
            if (this.m_Distance > 0f)
            {
                Vector3 direction = (position - source.transform.position).normalized;
                position -= direction * this.m_Distance;
            }
            
            return new Vector3(
                this.m_Axis.x >= 0.5f ? position.x : source.transform.position.x,
                this.m_Axis.y >= 0.5f ? position.y : source.transform.position.y,
                this.m_Axis.z >= 0.5f ? position.z : source.transform.position.z
            );
        }
    }
}