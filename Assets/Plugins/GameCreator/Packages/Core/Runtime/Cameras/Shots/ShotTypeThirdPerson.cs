using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Cameras
{
    [Title("Third Person")]
    [Category("Third Person")]
    [Image(typeof(IconShotThirdPerson), ColorTheme.Type.Blue)]
    
    [Description("Follows the target from a certain distance while allowing to orbit around it")]
    
    [Serializable]
    public class ShotTypeThirdPerson : TShotTypeLook
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private ShotSystemZoom m_ShotSystemZoom;
        [SerializeField] private ShotSystemOrbit m_ShotSystemOrbit;

        // PROPERTIES: ----------------------------------------------------------------------------

        public ShotSystemZoom Zoom => m_ShotSystemZoom;

        public override bool UseSmoothPosition => false;
        public override bool UseSmoothRotation => false;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public ShotTypeThirdPerson()
        {
            this.m_ShotSystemZoom = new ShotSystemZoom();
            this.m_ShotSystemOrbit = new ShotSystemOrbit();
            
            this.m_ShotSystems.Add(this.m_ShotSystemLook.Id, this.m_ShotSystemLook);
            this.m_ShotSystems.Add(this.m_ShotSystemZoom.Id, this.m_ShotSystemZoom);
            this.m_ShotSystems.Add(this.m_ShotSystemOrbit.Id, this.m_ShotSystemOrbit);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void AddRotation(float pitch, float yaw)
        {
            this.m_ShotSystemOrbit.Pitch += pitch;
            this.m_ShotSystemOrbit.Yaw += yaw;
        }

        // OVERRIDERS: ----------------------------------------------------------------------------

        protected override void OnBeforeAwake(ShotCamera shotCamera)
        {
            base.OnBeforeAwake(shotCamera);
            
            this.m_ShotSystemZoom?.OnAwake(this);
            this.m_ShotSystemOrbit?.OnAwake(this);
        }

        protected override void OnBeforeStart(ShotCamera shotCamera)
        {
            base.OnBeforeStart(shotCamera);
            
            this.m_ShotSystemZoom?.OnStart(this);
            this.m_ShotSystemOrbit?.OnStart(this);
        }

        protected override void OnBeforeDestroy(ShotCamera shotCamera)
        {
            base.OnBeforeDestroy(shotCamera);
            
            this.m_ShotSystemZoom?.OnDestroy(this);
            this.m_ShotSystemOrbit?.OnDestroy(this);
        }

        protected override void OnBeforeEnable(TCamera camera)
        {
            base.OnBeforeEnable(camera);
            
            this.m_ShotSystemZoom?.OnEnable(this, camera);
            this.m_ShotSystemOrbit?.OnEnable(this, camera);
        }

        protected override void OnBeforeDisable(TCamera camera)
        {
            base.OnBeforeDisable(camera);
            
            this.m_ShotSystemZoom?.OnDisable(this, camera);
            this.m_ShotSystemOrbit?.OnDisable(this, camera);
        }
        
        protected override void OnBeforeUpdate()
        {
            base.OnBeforeUpdate();

            this.m_ShotSystemZoom?.OnUpdate(this);
            this.m_ShotSystemOrbit.SyncWithZoom(this.m_ShotSystemZoom);
            
            this.m_ShotSystemOrbit?.OnUpdate(this);
        }
        
        // GIZMOS: --------------------------------------------------------------------------------
        
        public override void DrawGizmos(Transform transform)
        {
            base.DrawGizmos(transform);
            
            if (!Application.isPlaying) return;
            this.m_ShotSystemZoom.OnDrawGizmos(this, transform);
            this.m_ShotSystemOrbit.OnDrawGizmos(this, transform);
        }

        public override void DrawGizmosSelected(Transform transform)
        {
            base.DrawGizmosSelected(transform);
            
            if (!Application.isPlaying) return;
            this.m_ShotSystemZoom.OnDrawGizmosSelected(this, transform);
            this.m_ShotSystemOrbit.OnDrawGizmosSelected(this, transform);
        }
    }
}