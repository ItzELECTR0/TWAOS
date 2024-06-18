using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Serializable]
    public class ShotSystemAnimation : TShotSystem
    {
        public static readonly int ID = nameof(ShotSystemAnimation).GetHashCode();
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private float m_Duration = 3f;
        [SerializeField] private Easing.Type m_Easing = Easing.Type.QuadInOut;
        
        [SerializeField] private Bezier m_Path = new Bezier(
            new Vector3( 0f, 0f, -2f), // PointA 
            new Vector3( 0f, 0f,  2f), // PointB 
            new Vector3(-2f, 0f,  1f), // ControlA
            new Vector3(-2f, 0f, -1f)  // ControlB
        );
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        private float m_StartTime;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override int Id => ID;

        public float Duration
        {
            get => this.m_Duration;
            set => this.m_Duration = value;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override void OnEnable(TShotType shotType, TCamera camera)
        {
            base.OnEnable(shotType, camera);
            this.m_StartTime = shotType.ShotCamera.TimeMode.Time;
        }

        public override void OnUpdate(TShotType shotType)
        {
            base.OnUpdate(shotType);
            
            float elapsed = shotType.ShotCamera.TimeMode.Time - this.m_StartTime;
            float t = Easing.GetEase(this.m_Easing, 0f, 1f, elapsed / this.m_Duration);

            Vector3 position = this.m_Path.Get(t);
            shotType.Position = shotType.ShotCamera.transform.TransformPoint(position);
        }
        
        // GIZMOS: --------------------------------------------------------------------------------

        public override void OnDrawGizmosSelected(TShotType shotType, Transform transform)
        {
            base.OnDrawGizmosSelected(shotType, transform);
            this.DoDrawGizmos(shotType, GIZMOS_COLOR_ACTIVE, transform);
        }

        private void DoDrawGizmos(TShotType shotType, Color color, Transform transform)
        {
            Gizmos.color = color;
            this.m_Path.DrawGizmos(transform);
        }
    }
}