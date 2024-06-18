using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Title("Lock On")]
    [Category("Lock On")]
    [Image(typeof(IconShotLockOn), ColorTheme.Type.Blue)]
    
    [Description("Follows an object from a distance and tracks another one, so both are framed")]
    
    [Serializable]
    public class ShotTypeLockOn : TShotTypeLook
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private ShotSystemZoom m_ShotSystemZoom;
        [SerializeField] private ShotSystemLockOn m_ShotSystemLockOn;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private readonly Transform[] m_Ignore = new Transform[2];
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override Transform[] Ignore
        {
            get
            {
                this.m_Ignore[0] = this.Look.GetLookTarget(this);
                this.m_Ignore[1] = this.LockOn.GetAnchorTarget(this);

                return this.m_Ignore;
            }
        }

        public ShotSystemZoom Zoom => m_ShotSystemZoom;
        public ShotSystemLockOn LockOn => m_ShotSystemLockOn;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public ShotTypeLockOn()
        {
            this.m_ShotSystemLook = new ShotSystemLook(
                GetGameObjectInstance.Create(),
                GetDirectionVector3Zero.Create()
            );
            
            this.m_ShotSystemZoom = new ShotSystemZoom();
            this.m_ShotSystemLockOn = new ShotSystemLockOn();
            
            this.m_ShotSystems.Add(this.m_ShotSystemLook.Id, this.m_ShotSystemLook);
            this.m_ShotSystems.Add(this.m_ShotSystemZoom.Id, this.m_ShotSystemZoom);
            this.m_ShotSystems.Add(this.m_ShotSystemLockOn.Id, this.m_ShotSystemLockOn);
        }

        // OVERRIDERS: ----------------------------------------------------------------------------

        protected override void OnBeforeAwake(ShotCamera shotCamera)
        {
            base.OnBeforeAwake(shotCamera);
            
            this.m_ShotSystemZoom?.OnAwake(this);
            this.m_ShotSystemLockOn?.OnAwake(this);
        }

        protected override void OnBeforeStart(ShotCamera shotCamera)
        {
            base.OnBeforeStart(shotCamera);
            
            this.m_ShotSystemZoom?.OnStart(this);
            this.m_ShotSystemLockOn?.OnStart(this);
        }

        protected override void OnBeforeDestroy(ShotCamera shotCamera)
        {
            base.OnBeforeDestroy(shotCamera);
            
            this.m_ShotSystemZoom?.OnDestroy(this);
            this.m_ShotSystemLockOn?.OnDestroy(this);
        }

        protected override void OnBeforeEnable(TCamera camera)
        {
            base.OnBeforeEnable(camera);
            
            this.m_ShotSystemZoom?.OnEnable(this, camera);
            this.m_ShotSystemLockOn?.OnEnable(this, camera);
        }

        protected override void OnBeforeDisable(TCamera camera)
        {
            base.OnBeforeDisable(camera);
            
            this.m_ShotSystemZoom?.OnDisable(this, camera);
            this.m_ShotSystemLockOn?.OnDisable(this, camera);
        }
        
        protected override void OnBeforeUpdate()
        {
            base.OnBeforeUpdate();

            this.m_ShotSystemZoom?.OnUpdate(this);
            this.m_ShotSystemLockOn.SyncWithZoom(this.Args, this.Zoom);
            
            this.m_ShotSystemLockOn?.OnUpdate(this);
        }
        
        // GIZMOS: --------------------------------------------------------------------------------
        
        public override void DrawGizmos(Transform transform)
        {
            base.DrawGizmos(transform);
            
            if (!Application.isPlaying) return;
            this.m_ShotSystemZoom.OnDrawGizmos(this, transform);
            this.m_ShotSystemLockOn.OnDrawGizmos(this, transform);
        }

        public override void DrawGizmosSelected(Transform transform)
        {
            base.DrawGizmosSelected(transform);
            
            this.m_ShotSystemZoom.OnDrawGizmosSelected(this, transform);
            this.m_ShotSystemLockOn.OnDrawGizmosSelected(this, transform);
        }
    }
}