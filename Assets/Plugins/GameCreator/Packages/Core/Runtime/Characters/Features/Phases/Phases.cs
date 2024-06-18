using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public class Phases
    {
        public static readonly int[] HASH_PHASES =
        {
            Animator.StringToHash("Phase-0"),
            Animator.StringToHash("Phase-1"),
            Animator.StringToHash("Phase-2"),
            Animator.StringToHash("Phase-3"),
        };

        public static int Count => HASH_PHASES.Length;

        private const float GROUND_THRESHOLD = 0.1f;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private Animator m_Animator;
        
        [NonSerialized] private int m_LastChangeFrame = -1;

        [NonSerialized] private Phase[] m_Phases;

        // INITIALIZE METHODS: --------------------------------------------------------------------

        internal void Setup(Animator animator)
        {
            this.m_Animator = animator;
            
            this.m_Phases = new Phase[Count];
            for (int i = 0; i < Count; ++i)
            {
                this.m_Phases[i] = new Phase();
            }
        }
        
        // INTERNAL METHODS: ----------------------------------------------------------------------

        internal void Reset()
        {
            foreach (Phase phase in this.m_Phases)
            {
                phase.Reset();
            }
        }

        internal void Set(int phase, float value, float weight)
        {
            if (Time.frameCount != this.m_LastChangeFrame) this.Reset();

            this.m_Phases[phase].Add(value, weight);
            this.m_LastChangeFrame = Time.frameCount;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool IsGround(int index)
        {
            return this.Get(index) >= GROUND_THRESHOLD;
        }
        
        public float Get(int index)
        {
            float source = this.m_Animator != null
                ? this.m_Animator.GetFloat(HASH_PHASES[index])
                : 0f;
            
            return index >= 0 && index < Count ? this.m_Phases[index].Get(source) : 0f;
        }
    }
}