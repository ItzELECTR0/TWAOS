using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Serializable]
    public class ShotSystemAnchor : TShotSystem
    {
        public static readonly int ID = nameof(ShotSystemAnchor).GetHashCode();
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private PropertyGetGameObject m_Target = GetGameObjectPlayer.Create();
        [SerializeField] private PropertyGetDirection m_Offset = GetDirectionLocalValue.CreateTarget();

        [SerializeField] private PropertyGetDirection m_Distance = GetDirectionLocalValue.CreateTarget(
            new Vector3(0f, 0f, -3f)
        );
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override int Id => ID;

        public GameObject Target
        {
            set => this.m_Target = GetGameObjectInstance.Create(value);
        }
        
        public Vector3 Offset
        {
            set => this.m_Offset = GetDirectionLocalValue.CreateTarget(value);
        }
        
        public Vector3 Distance
        {
            set => this.m_Distance = GetDirectionLocalValue.CreateTarget(value);
        }

        // IMPLEMENTS: ----------------------------------------------------------------------------

        public override void OnUpdate(TShotType shotType)
        {
            base.OnUpdate(shotType);
            
            Vector3 target = this.GetTargetPosition(shotType);
            Vector3 source = this.GetAnchorPosition(shotType);
            Vector3 direction = target - source;
            
            shotType.Position = source;
            shotType.Rotation = Quaternion.LookRotation(direction);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public Transform GetTargetTransform(IShotType shotType)
        {
            GameObject target = this.m_Target.Get(shotType.ShotCamera);
            return target != null ? target.transform : null;
        }

        public Vector3 GetTargetPosition(IShotType shotType)
        {
            Transform target = this.GetTargetTransform(shotType);
            Vector3 offset = target != null
                ? this.m_Offset.Get(target)
                : shotType.ShotCamera.transform.TransformPoint(Vector3.forward);
            
            return target != null ? target.position + offset : default;
        }
        
        private Vector3 GetAnchorPosition(TShotType shotType)
        {
            Transform target = this.GetTargetTransform(shotType);
            if (target == null) return shotType.Position;
        
            Vector3 distance = this.m_Distance.Get(shotType.Args);
            return target.transform.position + distance;
        }

        // GIZMOS: --------------------------------------------------------------------------------

        public override void OnDrawGizmosSelected(TShotType shotType, Transform transform)
        {
            base.OnDrawGizmosSelected(shotType, transform);
            this.DoDrawGizmos(shotType, GIZMOS_COLOR_ACTIVE);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void DoDrawGizmos(TShotType shotType, Color color)
        {
            if (!Application.isPlaying) return;
            
            Gizmos.color = color;

            Vector3 target = this.GetTargetPosition(shotType);
            Vector3 source = this.GetAnchorPosition(shotType);

            Gizmos.DrawLine(source, target);
        }
    }
}