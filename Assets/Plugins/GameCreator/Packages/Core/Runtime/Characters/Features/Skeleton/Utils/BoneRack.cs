using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class BoneRack
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private Skeleton m_Skeleton;

        // EVENTS: --------------------------------------------------------------------------------

        public event Action EventChangeSkeleton;

        // PROPERTIES: ----------------------------------------------------------------------------

        public bool HasSkeleton => this.m_Skeleton != null;

        public Skeleton Skeleton
        {
            get => this.m_Skeleton;
            set
            {
                this.m_Skeleton = value;
                this.EventChangeSkeleton?.Invoke();
            }
        }

        // DRAW GIZMOS: ---------------------------------------------------------------------------
        
        internal void DrawGizmos(Animator animator)
        {
            if (animator == null) return;
            if (this.Skeleton == null) return;

            this.Skeleton.DrawGizmos(animator, Volumes.Display.Solid);
        }
    }
}