using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Title("Animation")]
    [Category("Animation")]
    [Image(typeof(IconShotAnimation), ColorTheme.Type.Blue)]
    
    [Description("Plays an animation where the Camera moves along a path")]
    
    [Serializable]
    public class ShotTypeAnimation : TShotTypeLook
    {
        [SerializeField] private ShotSystemAnimation m_ShotSystemAnimation;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public ShotSystemAnimation Animation => m_ShotSystemAnimation;

        public override Vector3 Position { get; set; }
        public override Quaternion Rotation { get; set; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public ShotTypeAnimation()
        {
            this.m_ShotSystemAnimation = new ShotSystemAnimation();
            
            this.m_ShotSystems.Add(this.m_ShotSystemLook.Id, this.m_ShotSystemLook);
            this.m_ShotSystems.Add(this.m_ShotSystemAnimation.Id, this.m_ShotSystemAnimation);
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        protected override void OnBeforeAwake(ShotCamera shotCamera)
        {
            base.OnBeforeAwake(shotCamera);

            this.Position = shotCamera.transform.position;
            this.Rotation = shotCamera.transform.rotation;
            
            this.m_ShotSystemAnimation.OnAwake(this);
        }

        protected override void OnBeforeEnable(TCamera camera)
        {
            base.OnBeforeEnable(camera);
            this.m_ShotSystemAnimation.OnEnable(this, camera);
        }

        protected override void OnBeforeUpdate()
        {
            base.OnBeforeUpdate();
            this.m_ShotSystemAnimation.OnUpdate(this);
        }
        
        // GIZMOS: --------------------------------------------------------------------------------

        public override void DrawGizmos(Transform transform)
        {
            base.DrawGizmos(transform);
            this.m_ShotSystemAnimation.OnDrawGizmos(this, transform);
        }

        public override void DrawGizmosSelected(Transform transform)
        {
            base.DrawGizmosSelected(transform);
            this.m_ShotSystemAnimation.OnDrawGizmosSelected(this, transform);
        }
    }
}