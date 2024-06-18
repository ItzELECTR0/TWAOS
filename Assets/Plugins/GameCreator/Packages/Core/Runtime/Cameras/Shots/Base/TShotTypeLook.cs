using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Serializable]
    public abstract class TShotTypeLook : TShotType
    {
        private const float OBSTACLE_MIN_DISTANCE = 0.5f;
        private const int OBSTACLE_LAYER_MASK = -1;
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] protected ShotSystemLook m_ShotSystemLook;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private readonly Transform[] m_Ignore = new Transform[1];
        [NonSerialized] private readonly RaycastHit[] m_ObstacleHits = new RaycastHit[10];

        // PROPERTIES: ----------------------------------------------------------------------------

        public ShotSystemLook Look => this.m_ShotSystemLook;

        public override Transform Target => this.m_ShotSystemLook.GetLookTarget(this);

        public override Args Args
        {
            get
            {
                this.m_Args ??= new Args(this.m_ShotCamera, null);
                this.m_Args.ChangeTarget(this.Look.GetLookTarget(this));
        
                return this.m_Args;
            }
        }

        public override Transform[] Ignore
        {
            get
            {
                this.m_Ignore[0] = this.Look.GetLookTarget(this);
                return this.m_Ignore;
            }
        }

        public override bool HasObstacle
        {
            get
            {
                Vector3 target = this.m_ShotSystemLook.GetLookPosition(this);
                Ray ray = new Ray(
                    this.Position,
                    target != default
                        ? target - this.Position
                        : this.Rotation * Vector3.forward
                );
                
                float distance = target != default
                    ? Vector3.Distance(this.Position, target)
                    : OBSTACLE_MIN_DISTANCE;

                int hits = Physics.RaycastNonAlloc(
                    ray, this.m_ObstacleHits, distance,
                    OBSTACLE_LAYER_MASK, QueryTriggerInteraction.Ignore
                );
                
                Transform[] ignoreTransforms = this.Ignore;

                for (int i = 0; i < hits; ++i)
                {
                    RaycastHit hit = this.m_ObstacleHits[i];
                    bool hasObstacle = true;

                    foreach (Transform transform in ignoreTransforms)
                    {
                        if (!hit.transform.IsChildOf(transform)) continue;
                        
                        hasObstacle = false;
                        break;
                    }

                    if (hasObstacle) return true;
                }
                
                return false;
            }
        }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        protected TShotTypeLook() : base()
        {
            this.m_ShotSystemLook = new ShotSystemLook();
        }
        
        // MAIN METHODS: --------------------------------------------------------------------------

        protected override void OnBeforeAwake(ShotCamera shotCamera)
        {
            base.OnBeforeAwake(shotCamera);
            this.Look.OnAwake(this);
        }

        protected override void OnBeforeStart(ShotCamera shotCamera)
        {
            base.OnBeforeStart(shotCamera);
            this.Look.OnStart(this);
        }

        protected override void OnBeforeDestroy(ShotCamera shotCamera)
        {
            base.OnBeforeDestroy(shotCamera);
            this.Look.OnDestroy(this);
        }

        protected override void OnAfterUpdate()
        {
            base.OnAfterUpdate();
            this.Look.OnUpdate(this);
        }

        protected override void OnBeforeDisable(TCamera camera)
        {
            base.OnBeforeDisable(camera);
            this.Look.OnDisable(this, camera);
        }

        protected override void OnBeforeEnable(TCamera camera)
        {
            base.OnBeforeEnable(camera);
            this.Look.OnEnable(this, camera);
        }

        // GIZMOS: --------------------------------------------------------------------------------

        public override void DrawGizmos(Transform transform)
        {
            this.Look.OnDrawGizmos(this, transform);
        }

        public override void DrawGizmosSelected(Transform transform)
        {
            this.Look.OnDrawGizmosSelected(this, transform);
        }
    }
}