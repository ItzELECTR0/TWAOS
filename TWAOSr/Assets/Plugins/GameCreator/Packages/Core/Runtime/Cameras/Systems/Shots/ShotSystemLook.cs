using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Serializable]
    public class ShotSystemLook : TShotSystem
    {
        public static readonly int ID = nameof(ShotSystemLook).GetHashCode();
        
        protected static readonly Vector3 GIZMO_SIZE_CUBE = Vector3.one * 0.1f;
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private bool m_IsActive = true;
        
        [SerializeField]
        private PropertyGetGameObject m_LookTarget = GetGameObjectPlayer.Create();

        [SerializeField] 
        private PropertyGetDirection m_LookOffset = GetDirectionVector3Zero.Create();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override int Id => ID;

        public bool IsActive
        {
            get => this.m_IsActive;
            set => this.m_IsActive = value;
        }

        public GameObject Target
        {
            set => this.m_LookTarget = GetGameObjectInstance.Create(value);
        }
        
        public Vector3 Offset
        {
            set => this.m_LookOffset = GetDirectionLocalValue.CreateSelf(value);
        }

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public ShotSystemLook() : base()
        { }

        public ShotSystemLook(PropertyGetGameObject target, PropertyGetDirection offset) : this()
        {
            this.m_LookTarget = target;
            this.m_LookOffset = offset;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Transform GetLookTarget(IShotType shotType)
        {
            if (!this.m_IsActive) return null;
            
            GameObject target = this.m_LookTarget.Get(shotType.ShotCamera);
            return target != null ? target.transform : null;
        }
        
        public Vector3 GetLookPosition(IShotType shotType)
        {
            Transform target = this.GetLookTarget(shotType);
            return target != null
                ? target.position + this.GetLookOffset(shotType)
                : default;
        }

        // IMPLEMENTS: ----------------------------------------------------------------------------
        
        public override void OnUpdate(TShotType shotType)
        {
            base.OnUpdate(shotType);
            
            if (!this.m_IsActive) return;
            
            Transform value = this.GetLookTarget(shotType);
            if (value == null || value == shotType.ShotCamera.transform) return;
            
            Vector3 direction = this.GetLookPosition(shotType) - shotType.Position; 
            shotType.Rotation = Quaternion.LookRotation(direction);
        }

        public override void OnDrawGizmosSelected(TShotType shotType, Transform transform)
        {
            base.OnDrawGizmosSelected(shotType, transform);
            this.DoDrawGizmos(shotType, GIZMOS_COLOR_ACTIVE);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private Vector3 GetLookOffset(IShotType shotType)
        {
            Transform target = this.GetLookTarget(shotType);
            return target != null 
                ? this.m_LookOffset.Get(target) 
                : default;
        }
        
        // GIZMO METHODS: -------------------------------------------------------------------------
        
        private void DoDrawGizmos(TShotType shotType, Color color)
        {
            if (!Application.isPlaying) return;
            
            Gizmos.color = color;
            if (!this.m_IsActive) return;
            
            Transform target = this.GetLookTarget(shotType);
            if (target == null || target.position == shotType.Position) return;

            Vector3 look = this.GetLookPosition(shotType);
            
            Gizmos.DrawWireCube(look, GIZMO_SIZE_CUBE);
            Gizmos.DrawLine(shotType.Position, look);
        }
    }
}