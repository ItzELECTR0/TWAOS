using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class Volumes : TPolymorphicList<TVolume>
    {
        public enum Display
        {
            None,
            Outline,
            Solid
        }
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeReference] private IVolume[] m_Volumes;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override int Length => this.m_Volumes.Length;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public Volumes()
        {
            this.m_Volumes = Array.Empty<IVolume>();
        }

        public Volumes(params IVolume[] volumes)
        {
            List<IVolume> candidates = new List<IVolume>();
            foreach (IVolume volume in volumes)
            {
                if (volume == null) continue;
                candidates.Add(volume);
            }
            
            this.m_Volumes = candidates.ToArray();
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public GameObject[] Update(Animator animator, float mass, Skeleton skeleton)
        {
            GameObject[] targets = new GameObject[this.m_Volumes.Length];
            float weightDistribution = this.GetWeightDistribution();
            
            for (int i = 0; i < this.m_Volumes.Length; ++i)
            {
                float boneMass = mass * (this.m_Volumes[i].Weight / weightDistribution);
                targets[i] = this.m_Volumes[i].UpdatePass1Physics(animator, boneMass, skeleton);
            }
            
            for (int i = 0; i < this.m_Volumes.Length; ++i)
            {
                if (targets[i] == null) continue;
                this.m_Volumes[i].UpdatePass2Joints(targets[i], animator, skeleton);
            }

            return targets;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private float GetWeightDistribution()
        {
            float distribution = 0f;
            foreach (IVolume volume in this.m_Volumes)
            {
                distribution += volume.Weight;
            }

            return distribution;
        }

        // DRAW GIZMOS: ---------------------------------------------------------------------------

        public void DrawGizmos(Animator animator, Display display)
        {
            Gizmos.color = new Color(1, 0f, 0f, 0.5f);
            foreach (IVolume volume in this.m_Volumes)
            {
                volume.DrawGizmos(animator, display);
            }
        }
    }
}