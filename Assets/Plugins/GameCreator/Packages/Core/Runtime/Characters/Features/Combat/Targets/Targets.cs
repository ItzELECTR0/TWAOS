using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public class Targets
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private GameObject m_Primary;
        [NonSerialized] private readonly List<GameObject> m_List = new List<GameObject>();

        // PROPERTIES: ----------------------------------------------------------------------------

        public GameObject Primary
        {
            get => this.m_Primary;
            set
            {
                if (this.m_Primary == value) return;
                
                this.m_Primary = value;
                if (value != null && !this.m_List.Contains(value))
                {
                    this.m_List.Add(value);
                }
                
                this.EventChangeTarget?.Invoke(this.m_Primary);
            }
        }

        public List<GameObject> List
        {
            get
            {
                this.CleanNulls();
                return this.m_List;
            }
        }

        // EVENTS: --------------------------------------------------------------------------------

        public event Action<GameObject> EventChangeTarget;
        
        public event Action<GameObject> EventCandidateAdd;
        public event Action<GameObject> EventCandidateRemove;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void AddCandidate(GameObject candidate)
        {
            this.CleanNulls();
            
            if (candidate == null) return;
            if (this.m_List.Contains(candidate)) return;

            this.m_List.Add(candidate);
            this.EventCandidateAdd?.Invoke(candidate);
        }
        
        public void RemoveCandidate(GameObject candidate)
        {
            this.CleanNulls();
            
            if (candidate == null) return;
            if (!this.m_List.Contains(candidate)) return;

            this.m_List.Remove(candidate);
            if (this.m_Primary == candidate)
            {
                this.Primary = null;
            }
            
            this.EventCandidateRemove?.Invoke(candidate);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void CleanNulls()
        {
            for (int i = this.m_List.Count - 1; i >= 0; --i)
            {
                if (this.m_List[i] != null) continue;
                this.m_List.RemoveAt(i);
            }
        }
    }
}