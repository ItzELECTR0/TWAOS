using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Serializable]
    public class ShotSystemTrack : TShotSystem
    {
        public static readonly int ID = nameof(ShotSystemTrack).GetHashCode();
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField]
        private PropertyGetGameObject m_Target = GetGameObjectPlayer.Create();

        [SerializeField]
        private PropertyGetDirection m_Offset = GetDirectionVector3Zero.Create();
        
        [SerializeField] private Bezier m_Track = new Bezier(
            new Vector3( 0f, 0f, -2f), // PointA 
            new Vector3( 0f, 0f,  2f), // PointB 
            new Vector3(-2f, 0f,  1f), // ControlA
            new Vector3(-2f, 0f, -1f)  // ControlB
        );

        [SerializeField] private Segment m_RelativeTo = new Segment(
            new Vector3(0f, -2f, -3f),
            new Vector3(0f, -2f,  3f)
        );
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override int Id => ID;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override void OnUpdate(TShotType shotType)
        {
            base.OnUpdate(shotType);
            
            Vector3 targetPosition = this.GetTarget(shotType);
            Vector3 toA = shotType.ShotCamera.transform.TransformPoint(this.m_RelativeTo.PointA);
            Vector3 toB = shotType.ShotCamera.transform.TransformPoint(this.m_RelativeTo.PointB);

            Vector3 toTarget = targetPosition - toA;

            Vector3 toRelative = toB - toA;
            Vector3 vectorProjection = Vector3.Project(toTarget, toRelative);

            float sign = Mathf.Sign(Vector3.Dot(vectorProjection, toRelative));
            float t = vectorProjection.magnitude / toRelative.magnitude * sign;

            Vector3 localPosition = this.m_Track.Get(t);
            shotType.Position = shotType.ShotCamera.transform.TransformPoint(localPosition);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private Vector3 GetTarget(TShotType shotType)
        {
            GameObject target = this.m_Target.Get(shotType.ShotCamera);
            Vector3 position = target != null ? target.transform.position : Vector3.zero;
            Vector3 offset = target != null ? this.m_Offset.Get(target) : Vector3.zero;
            return position + offset;
        }
        
        // GIZMOS: --------------------------------------------------------------------------------

        public override void OnDrawGizmosSelected(TShotType shotType, Transform transform)
        {
            base.OnDrawGizmosSelected(shotType, transform);
            this.DoDrawGizmos(shotType, GIZMOS_COLOR_ACTIVE);
        }

        private void DoDrawGizmos(TShotType shotType, Color color)
        {
            Gizmos.color = color;

            this.m_RelativeTo.DrawGizmos(shotType.ShotCamera.transform);
            this.m_Track.DrawGizmos(shotType.ShotCamera.transform);
        }
    }
}