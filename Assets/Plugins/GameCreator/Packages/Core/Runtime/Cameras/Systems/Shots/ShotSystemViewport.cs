using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Serializable]
    public class ShotSystemViewport : TShotSystem
    {
        public static readonly int ID = nameof(ShotSystemViewport).GetHashCode();

        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private EnablerProjection m_Projection = new EnablerProjection();
        [SerializeField] private EnablerFloat m_FieldOfView = new EnablerFloat(false, 60f);
        [SerializeField] private EnablerFloat m_OrthographicSize = new EnablerFloat(false, 5f);
        
        [SerializeField] private EnablerFloat m_ClipPlaneNear = new EnablerFloat(false, 0.3f);
        [SerializeField] private EnablerFloat m_ClipPlaneFar = new EnablerFloat(false, 1000f);

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override int Id => ID;

        public bool ChangeProjection => this.m_Projection.IsEnabled;
        public bool ChangeFieldOfView => this.m_FieldOfView.IsEnabled;
        public bool ChangeOrthographicSize => this.m_OrthographicSize.IsEnabled;
        
        public bool ChangeClipPlaneNear => this.m_ClipPlaneNear.IsEnabled;
        public bool ChangeClipPlaneFar => this.m_ClipPlaneFar.IsEnabled;

        public bool Projection => this.m_Projection.Value == EnablerProjection.Type.Orthographic;
        public float FieldOfView => this.m_FieldOfView.Value;
        public float OrthographicSize => this.m_OrthographicSize.Value;

        public float ClipPlaneNear => this.m_ClipPlaneNear.Value;
        public float ClipPlaneFar => this.m_ClipPlaneFar.Value;
    }
}