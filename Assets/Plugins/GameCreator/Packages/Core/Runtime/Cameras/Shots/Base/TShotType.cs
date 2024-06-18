using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameCreator.Runtime.Cameras
{
    [Serializable]
    public abstract class TShotType : IShotType
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] protected ShotSystemViewport m_ShotSystemViewport;
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] protected ShotCamera m_ShotCamera;
        [NonSerialized] protected Transform m_Transform;

        [NonSerialized] protected Args m_Args;

        [NonSerialized] protected Dictionary<int, IShotSystem> m_ShotSystems;

        // PROPERTIES: ----------------------------------------------------------------------------

        public virtual Vector3 Position
        {
            get => this.m_Transform != null ? this.m_Transform.position : default;
            set => this.m_Transform.position = value;
        }
        
        public virtual Quaternion Rotation
        {
            get => this.m_Transform != null ? this.m_Transform.rotation : default;
            set => this.m_Transform.rotation = value;
        }

        public bool IsActive { get; private set; }
        public abstract Transform[] Ignore { get; }

        public abstract Args Args { get; }

        public ShotCamera ShotCamera => this.m_ShotCamera;
        public Transform Transform => this.m_Transform;

        public virtual bool UseSmoothPosition => true;
        public virtual bool UseSmoothRotation => true;

        public IShotSystem[] ShotSystems
        {
            get
            {
                List<IShotSystem> shotSystems = new List<IShotSystem>(); 
                foreach (IShotSystem shotSystem in this.m_ShotSystems.Values)
                {
                    shotSystems.Add(shotSystem);
                }

                return shotSystems.ToArray();
            }            
        }

        public virtual bool HasObstacle => false;

        public abstract Transform Target { get; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        protected TShotType()
        {
            this.m_ShotSystems = new Dictionary<int, IShotSystem>();
            
            this.m_ShotSystemViewport = new ShotSystemViewport();
            this.m_ShotSystems.Add(this.m_ShotSystemViewport.Id, this.m_ShotSystemViewport);
        }
        
        // MAIN METHODS: --------------------------------------------------------------------------

        public void Awake(ShotCamera shotCamera)
        {
            this.m_ShotCamera = shotCamera;
            this.m_Args = new Args(this.m_ShotCamera);

            this.m_Transform = this.m_ShotCamera.transform;
            this.OnBeforeAwake(shotCamera);
            this.OnAfterAwake(shotCamera);
        }

        public void Start(ShotCamera shotCamera)
        {
            this.OnBeforeStart(shotCamera);
            this.OnAfterStart(shotCamera);
        }

        public void Destroy(ShotCamera shotCamera)
        {
            this.OnBeforeDestroy(shotCamera);
            this.OnAfterDestroy(shotCamera);
        }

        public void Update()
        {
            this.OnBeforeUpdate();
            this.OnAfterUpdate();
        }

        public void OnEnable(TCamera camera)
        {
            this.IsActive = true;

            this.OnBeforeEnable(camera);
            this.OnAfterEnable(camera);
        }
        
        public void OnDisable(TCamera camera)
        {
            this.IsActive = false;
            this.OnBeforeDisable(camera);
            this.OnAfterDisable(camera);
        }

        // INTERFACE METHODS: ---------------------------------------------------------------------

        protected virtual void OnBeforeAwake(ShotCamera shotCamera) { }
        protected virtual void OnAfterAwake(ShotCamera shotCamera) { }

        protected virtual void OnBeforeStart(ShotCamera shotCamera) { }
        protected virtual void OnAfterStart(ShotCamera shotCamera) { }
        
        protected virtual void OnBeforeDestroy(ShotCamera shotCamera) { }
        protected virtual void OnAfterDestroy(ShotCamera shotCamera) { }
        
        protected virtual void OnBeforeUpdate() { }
        protected virtual void OnAfterUpdate() { }

        protected virtual void OnBeforeDisable(TCamera camera) { }
        protected virtual void OnAfterDisable(TCamera camera) { }

        protected virtual void OnBeforeEnable(TCamera camera) { }
        protected virtual void OnAfterEnable(TCamera camera) { }

        // GIZMOS: --------------------------------------------------------------------------------

        public virtual void DrawGizmos(Transform transform)
        { }

        public virtual void DrawGizmosSelected(Transform transform)
        { }
        
        // GETTERS: -------------------------------------------------------------------------------

        public IShotSystem GetSystem(int systemID)
        {
            return this.m_ShotSystems.TryGetValue(systemID, out IShotSystem system)
                ? system
                : null;
        }
    }
}