using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Serializable]
    public class AbilitiesCatalogue
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private Dictionary<IdString, Ability> m_Map;
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private Ability[] m_Abilities = Array.Empty<Ability>();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public Ability[] List => this.m_Abilities;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Ability Get(IdString abilityID)
        {
            this.RequireInitialize();
            return this.m_Map.TryGetValue(abilityID, out Ability ability) ? ability : null;
        }

        private void RequireInitialize()
        {
            if (this.m_Map != null) return;
            
            this.m_Map = new Dictionary<IdString, Ability>();
            foreach (Ability ability in this.m_Abilities) this.m_Map[ability.ID] = ability;
        }
        
        // INTERNAL METHODS: ----------------------------------------------------------------------
        
        internal void Set(Ability[] abilities)
        {
            this.m_Abilities = abilities;
        }
    }
}