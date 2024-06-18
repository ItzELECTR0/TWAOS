using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    
    [Icon(RuntimePaths.GIZMOS + "GizmoStaging.png")]
    
    public class StagingGizmos : MonoBehaviour
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private ScriptableObject m_Asset;
        [NonSerialized] private IStageGizmos m_Gizmos;

        // PROPERTIES: ----------------------------------------------------------------------------

        public Animator Animator => this.GetComponentInChildren<Animator>();
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static StagingGizmos Bind<T>(GameObject target, T asset) where T : ScriptableObject, IStageGizmos
        {
            StagingGizmos component = target.AddComponent<StagingGizmos>();

            component.m_Asset = asset;
            component.m_Gizmos = asset;
            
            component.hideFlags = HideFlags.DontSave;
            return component;
        }

        public void SelectAsset()
        {
            #if UNITY_EDITOR
            
            if (this.m_Asset == null) return;
            UnityEditor.Selection.activeObject = this.m_Asset;
            
            #endif
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private T GetAsset<T>() where T : ScriptableObject
        {
            return this.m_Asset as T;
        }
        
        // GIZMOS: --------------------------------------------------------------------------------
        
        #if UNITY_EDITOR

        private const UnityEditor.GizmoType GIZMOS =
            UnityEditor.GizmoType.InSelectionHierarchy |
            UnityEditor.GizmoType.NotInSelectionHierarchy;
        
        [UnityEditor.DrawGizmo(GIZMOS)]
        private static void DrawGizmos(StagingGizmos component, UnityEditor.GizmoType gizmoType)
        {
            if (component.m_Asset == null) return;
            component.m_Gizmos?.StageGizmos(component);
        }
        
        #endif
    }
}