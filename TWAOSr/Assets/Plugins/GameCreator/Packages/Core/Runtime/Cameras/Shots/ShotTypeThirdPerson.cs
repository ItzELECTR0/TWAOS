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
    public class ShotTypeThirdPerson : TShotType
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private ShotSystemZoom m_Zoom;
        [SerializeField] private ShotSystemThirdPerson m_ThirdPerson;
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private readonly Transform[] m_Ignore = new Transform[1];

        // PROPERTIES: ----------------------------------------------------------------------------

        public ShotSystemZoom Zoom => m_Zoom;
        
        public override bool UseSmoothPosition => false;
        public override bool UseSmoothRotation => false;
        
        public override Args Args
        {
            get
            {
                this.m_Args ??= new Args(this.m_ShotCamera, null);
                this.m_Args.ChangeTarget(this.m_ThirdPerson.Pivot);
                
                return this.m_Args;
            }
        }
        
        public override Transform[] Ignore
        {
            get
            {
                GameObject pivot = this.m_ThirdPerson.Pivot;
                this.m_Ignore[0] = pivot != null ? pivot.transform : null;
                
                return this.m_Ignore;
            }
        }

        public override bool HasTarget => this.m_ThirdPerson.Pivot != null;
        public override Vector3 Target => this.m_ThirdPerson.PivotPosition;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public ShotTypeThirdPerson()
        {
            this.m_Zoom = new ShotSystemZoom();
            this.m_ThirdPerson = new ShotSystemThirdPerson();
            
            this.m_ShotSystems.Add(this.m_Zoom.Id, this.m_Zoom);
            this.m_ShotSystems.Add(this.m_ThirdPerson.Id, this.m_ThirdPerson);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void AddRotation(float pitch, float yaw)
        {
            this.m_ThirdPerson.Pitch += pitch;
            this.m_ThirdPerson.Yaw += yaw;
        }

        // OVERRIDERS: ----------------------------------------------------------------------------

        protected override void OnBeforeAwake(ShotCamera shotCamera)
        {
            base.OnBeforeAwake(shotCamera);
            
            this.m_Zoom?.OnAwake(this);
            this.m_ThirdPerson?.OnAwake(this);
        }

        protected override void OnBeforeStart(ShotCamera shotCamera)
        {
            base.OnBeforeStart(shotCamera);
            
            this.m_Zoom?.OnStart(this);
            this.m_ThirdPerson?.OnStart(this);
        }

        protected override void OnBeforeDestroy(ShotCamera shotCamera)
        {
            base.OnBeforeDestroy(shotCamera);
            
            this.m_Zoom?.OnDestroy(this);
            this.m_ThirdPerson?.OnDestroy(this);
        }

        protected override void OnBeforeEnable(TCamera camera)
        {
            base.OnBeforeEnable(camera);
            
            this.m_Zoom?.OnEnable(this, camera);
            this.m_ThirdPerson?.OnEnable(this, camera);
        }

        protected override void OnBeforeDisable(TCamera camera)
        {
            base.OnBeforeDisable(camera);
            
            this.m_Zoom?.OnDisable(this, camera);
            this.m_ThirdPerson?.OnDisable(this, camera);
        }
        
        protected override void OnBeforeUpdate()
        {
            base.OnBeforeUpdate();

            this.m_Recoil.Update(out float pitch, out float yaw);
            this.AddRotation(pitch, yaw);
            
            this.m_Zoom?.OnUpdate(this);
            this.m_ThirdPerson?.OnUpdate(this);
        }
        
        // GIZMOS: --------------------------------------------------------------------------------
        
        public override void DrawGizmos(Transform transform)
        {
            base.DrawGizmos(transform);
            if (!Application.isPlaying) return;
            
            this.m_Zoom.OnDrawGizmos(this, transform);
            this.m_ThirdPerson?.OnDrawGizmos(this, transform);
        }

        public override void DrawGizmosSelected(Transform transform)
        {
            base.DrawGizmosSelected(transform);
            
            if (!Application.isPlaying) return;
            
            this.m_Zoom.OnDrawGizmosSelected(this, transform);
            this.m_ThirdPerson?.OnDrawGizmosSelected(this, transform);
        }
    }
}