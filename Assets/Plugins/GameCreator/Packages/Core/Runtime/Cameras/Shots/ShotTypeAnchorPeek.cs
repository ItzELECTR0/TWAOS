using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Title("Anchor Peek")]
    [Category("Anchor Peek")]
    [Image(typeof(IconShotAnchor), ColorTheme.Type.Blue)]
    
    [Description("Anchors to an object and allows to pan and tilt the Shot up, down and to the sides")]
    
    [Serializable]
    public class ShotTypeAnchorPeek : TShotType
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private ShotSystemAnchor m_ShotSystemAnchor;
        [SerializeField] private ShotSystemPeek m_ShotSystemPeek;

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private readonly Transform[] m_Ignore = new Transform[1];

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public ShotSystemAnchor Anchor => this.m_ShotSystemAnchor;
        public ShotSystemPeek Peek => this.m_ShotSystemPeek;
        
        public override Args Args
        {
            get
            {
                this.m_Args ??= new Args(this.m_ShotCamera, null);
                this.m_Args.ChangeTarget(this.Anchor.GetTargetTransform(this));
        
                return this.m_Args;
            }
        }

        public override Transform[] Ignore
        {
            get
            {
                this.m_Ignore[0] = this.Anchor.GetTargetTransform(this);
                return this.m_Ignore;
            }
        }

        public override Transform Target => this.Anchor.GetTargetTransform(this);

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public ShotTypeAnchorPeek()
        {
            this.m_ShotSystemAnchor = new ShotSystemAnchor();
            this.m_ShotSystemPeek = new ShotSystemPeek();
            
            this.m_ShotSystems.Add(this.m_ShotSystemAnchor.Id, this.m_ShotSystemAnchor);
            this.m_ShotSystems.Add(this.m_ShotSystemPeek.Id, this.m_ShotSystemPeek);
        }

        // OVERRIDERS: ----------------------------------------------------------------------------

        protected override void OnBeforeAwake(ShotCamera shotCamera)
        {
            base.OnBeforeAwake(shotCamera);
            
            this.m_ShotSystemAnchor?.OnAwake(this);
            this.m_ShotSystemPeek?.OnAwake(this);
        }

        protected override void OnBeforeStart(ShotCamera shotCamera)
        {
            base.OnBeforeStart(shotCamera);
            
            this.m_ShotSystemAnchor?.OnStart(this);
            this.m_ShotSystemPeek?.OnStart(this);
        }

        protected override void OnBeforeDestroy(ShotCamera shotCamera)
        {
            base.OnBeforeDestroy(shotCamera);
            
            this.m_ShotSystemAnchor?.OnDestroy(this);
            this.m_ShotSystemPeek?.OnDestroy(this);
        }

        protected override void OnBeforeEnable(TCamera camera)
        {
            base.OnBeforeEnable(camera);
            
            this.m_ShotSystemAnchor?.OnEnable(this, camera);
            this.m_ShotSystemPeek?.OnEnable(this, camera);
        }

        protected override void OnBeforeDisable(TCamera camera)
        {
            base.OnBeforeDisable(camera);
            
            this.m_ShotSystemAnchor?.OnDisable(this, camera);
            this.m_ShotSystemPeek?.OnDisable(this, camera);
        }

        protected override void OnBeforeUpdate()
        {
            base.OnBeforeUpdate();
            
            this.m_ShotSystemAnchor?.OnUpdate(this);
            this.m_ShotSystemPeek?.OnUpdate(this);
        }

        // GIZMOS: --------------------------------------------------------------------------------
        
        public override void DrawGizmos(Transform transform)
        {
            base.DrawGizmos(transform);
            
            this.m_ShotSystemAnchor.OnDrawGizmos(this, transform);
            this.m_ShotSystemPeek?.OnDrawGizmos(this, transform);
        }

        public override void DrawGizmosSelected(Transform transform)
        {
            base.DrawGizmosSelected(transform);
            
            this.m_ShotSystemAnchor.OnDrawGizmosSelected(this, transform);
            this.m_ShotSystemPeek?.OnDrawGizmosSelected(this, transform);
        }
    }
}