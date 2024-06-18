using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [AddComponentMenu("Game Creator/Cameras/Shot Camera")]
    [Icon(RuntimePaths.GIZMOS + "GizmoShot.png")]
    
    [DefaultExecutionOrder(ApplicationManager.EXECUTION_ORDER_DEFAULT)]
    
    [Serializable]
    public class ShotCamera : MonoBehaviour
    {
        public enum Clipping
        {
            AvoidClipping,
            ClipThrough
        }
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private bool m_IsMainShot;
        [SerializeField] private TimeMode m_TimeMode;
        [SerializeField] private Clipping m_Clipping = Clipping.AvoidClipping; 
        
        [SerializeReference] private IShotType m_ShotType = new ShotTypeFixed();

        // PROPERTIES: ----------------------------------------------------------------------------

        public IShotType ShotType => this.m_ShotType;

        public bool IsMainShot => this.m_IsMainShot;
        public bool AvoidClipping => this.m_Clipping == Clipping.AvoidClipping;

        public Vector3 Position => this.m_ShotType?.Position ?? transform.position;
        public Quaternion Rotation => this.m_ShotType?.Rotation ?? transform.rotation;
        
        public Transform Target => this.m_ShotType.Target;

        public Transform[] Ignore => this.m_ShotType.Ignore;

        public virtual bool UseSmoothPosition => this.m_ShotType?.UseSmoothPosition ?? false;
        public virtual bool UseSmoothRotation => this.m_ShotType?.UseSmoothRotation ?? false;

        public TimeMode TimeMode
        {
            get => this.m_TimeMode;
            set => this.m_TimeMode = value;
        }

        public bool HasObstacle => this.m_ShotType.HasObstacle;
        
        // EVENTS: --------------------------------------------------------------------------------

        public event Action<TCamera> EventChangeTo;
        public event Action<TCamera> EventChangeFrom;

        // INITIALIZERS: --------------------------------------------------------------------------

        protected virtual void Awake()
        {
            if (this.m_IsMainShot) ShortcutMainShot.Change(this);
            this.m_ShotType?.Awake(this);
        }

        protected virtual void Start()
        {
            this.m_ShotType?.Start(this);
        }

        protected void OnDestroy()
        {
            this.m_ShotType?.Destroy(this);
        }

        // UPDATE METHODS: ------------------------------------------------------------------------

        protected virtual void LateUpdate()
        {
            this.m_ShotType?.Update();
        }

        private void OnDrawGizmos()
        {
            #if UNITY_EDITOR
            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this.gameObject)) return;
            #endif
            
            this.m_ShotType?.DrawGizmos(this.transform);
        }
        
        private void OnDrawGizmosSelected()
        {
            #if UNITY_EDITOR
            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this.gameObject)) return;
            #endif
            
            this.m_ShotType?.DrawGizmosSelected(this.transform);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public virtual void OnEnableShot(TCamera cameraSystem)
        {
            this.m_ShotType?.OnEnable(cameraSystem);
            this.m_ShotType?.Update();
            this.EventChangeTo?.Invoke(cameraSystem);
        }

        public virtual void OnDisableShot(TCamera cameraSystem)
        {
            this.m_ShotType?.OnDisable(cameraSystem);
            this.EventChangeFrom?.Invoke(cameraSystem);
        }
    }
}