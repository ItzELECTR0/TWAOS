using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [HelpURL("https://docs.gamecreator.io/gamecreator/characters/markers")]
    [AddComponentMenu("Game Creator/Characters/Marker", 200)]
    
    [DisallowMultipleComponent]
    
    [Icon(RuntimePaths.GIZMOS + "GizmoMarker.png")]
	public class Marker : MonoBehaviour, ISpatialHash
	{
        #if UNITY_EDITOR
        
        public const string KEY_MARKER_CAPSULE_HEIGHT = "gc:marker-capsule-height";
        public const string KEY_MARKER_CAPSULE_RADIUS = "gc:marker-capsule-radius";
        
        #endif
        
        // STATIC: --------------------------------------------------------------------------------

        [field: NonSerialized]
        private static Dictionary<IdString, Marker> Markers { get; set; } = new Dictionary<IdString, Marker>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemsInit()
        {
            Markers = new Dictionary<IdString, Marker>();
        }
        
        private static readonly Color COLOR_GIZMO_CAPSULE = new Color(
            Color.yellow.r,
            Color.yellow.g, 
            Color.yellow.b,
            0.25f
        ); 
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private float m_StopDistance = 0.01f;
        [SerializeReference] private TMarkerType m_MarkerType = new MarkerTypeDirection();
        
        [SerializeField] private UniqueID m_UniqueID = new UniqueID();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public float StopDistance => Mathf.Max(0f, this.m_StopDistance);

        // INITIALIZERS: --------------------------------------------------------------------------

        private void Awake()
        {
            SpatialHashMarkers.Insert(this);
            Markers[this.m_UniqueID.Get] = this;
        }

        private void OnDestroy()
        {
            SpatialHashMarkers.Remove(this);
            Markers.Remove(this.m_UniqueID.Get);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool IsWithinRange(Vector3 target, float stopThreshold = 0f)
        {
            float threshold = Mathf.Max(this.m_StopDistance, stopThreshold);
            return Vector3.Distance(this.transform.position, target) <= threshold;
        }

        public Vector3 GetPosition(GameObject user)
        {
            return this.m_MarkerType.GetPosition(this, user);
        }
        
        public Vector3 GetDirection(GameObject user)
        {
            return this.m_MarkerType.GetDirection(this, user);
        }

        public Quaternion GetRotation(GameObject user)
        {
            Vector3 direction = this.GetDirection(user);
            return Quaternion.LookRotation(direction);
        }
        
        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static Marker GetMarkerByID(string makerId)
        {
            IdString id = new IdString(makerId);
            return GetMarkerByID(id);
        }
        
        public static Marker GetMarkerByID(IdString markerId)
        {
            return Markers.TryGetValue(markerId, out Marker marker)
                ? marker
                : null;
        }

        // GIZMOS: --------------------------------------------------------------------------------

        private void OnDrawGizmos()
        {
            #if UNITY_EDITOR
            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this.gameObject)) return;
            #endif
            
            Vector3 position = transform.position + Vector3.up * 0.01f;

            Gizmos.color = Color.yellow;
            this.m_MarkerType?.OnDrawGizmos(this);
            
            GizmosExtension.Cross(position, GizmosExtension.CrossDirection.Upwards, 0.05f);
            GizmosExtension.Circle(position, 0.05f);

            if (this.m_StopDistance >= 0.2f)
            {
                GizmosExtension.Circle(position, this.m_StopDistance);
            }
        }

        private void OnDrawGizmosSelected()
        {
            #if UNITY_EDITOR
            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this.gameObject)) return;
            #endif
            
            Gizmos.color = COLOR_GIZMO_CAPSULE;
            float radius = 0.2f;
            float height = 2f;
            
            #if UNITY_EDITOR
            radius = UnityEditor.EditorPrefs.GetFloat(KEY_MARKER_CAPSULE_RADIUS, 0.2f);
            height = UnityEditor.EditorPrefs.GetFloat(KEY_MARKER_CAPSULE_HEIGHT, 2f);
            #endif
            
            Vector3 position = transform.position + Vector3.up * 0.01f;
            GizmosExtension.Cylinder(position, height, radius);
        }
    }
}