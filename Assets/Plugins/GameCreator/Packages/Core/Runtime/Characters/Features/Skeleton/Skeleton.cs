using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [CreateAssetMenu(
        fileName = "My Skeleton",
        menuName = "Game Creator/Characters/Skeleton",
        order = 50
    )]
    [Icon(RuntimePaths.GIZMOS + "GizmoSkeleton.png")]
    
    [Serializable]
    public class Skeleton : ScriptableObject, IStageGizmos
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] 
        private PhysicsMaterial m_Material;

        [SerializeField]
        private CollisionDetectionMode m_CollisionDetection = CollisionDetectionMode.Discrete;

        [SerializeReference] 
        private Volumes m_Volumes = new Volumes();

        // PROPERTIES: ----------------------------------------------------------------------------

        public PhysicsMaterial Material => m_Material;
        public CollisionDetectionMode CollisionDetection => m_CollisionDetection;

        public bool IsEmpty => this.m_Volumes.Length == 0;
        
        [field: SerializeField] public string EditorModelPath { get; set; }

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public GameObject[] Refresh(Character character)
        {
            if (character == null) return Array.Empty<GameObject>();

            Animator animator = character.Animim.Animator;
            return animator != null 
                ? this.Refresh(animator, character.Motion.Mass)
                : Array.Empty<GameObject>();
        }
        
        public GameObject[] Refresh(Animator animator, float mass)
        {
            return this.m_Volumes.Update(animator, mass, this);
        }
        
        // DRAW GIZMOS: ---------------------------------------------------------------------------

        public void DrawGizmos(Animator animator, Volumes.Display display)
        {
            this.m_Volumes.DrawGizmos(animator, display);
        }

        public void StageGizmos(StagingGizmos stagingGizmos)
        {
            Animator animator = stagingGizmos.Animator;
            if (animator == null) return;
            
            this.DrawGizmos(animator, Volumes.Display.Solid);
        }
    }
}
