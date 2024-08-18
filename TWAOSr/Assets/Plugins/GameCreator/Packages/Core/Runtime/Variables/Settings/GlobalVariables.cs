using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public class GlobalVariables
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private Dictionary<IdString, GlobalNameVariables> m_MapNameVariables;
        [NonSerialized] private Dictionary<IdString, GlobalListVariables> m_MapListVariables;
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private GlobalNameVariables[] m_NameVariables = Array.Empty<GlobalNameVariables>();
        [SerializeField] private GlobalListVariables[] m_ListVariables = Array.Empty<GlobalListVariables>();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public GlobalNameVariables[] NameVariables => this.m_NameVariables;
        public GlobalListVariables[] ListVariables => this.m_ListVariables;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public GlobalNameVariables GetNameVariablesAsset(IdString itemID)
        {
            this.RequireInitialize();
            return this.m_MapNameVariables.TryGetValue(itemID, out GlobalNameVariables variable)
                ? variable
                : null;
        }
        
        public GlobalListVariables GetListVariablesAsset(IdString itemID)
        {
            this.RequireInitialize();
            return this.m_MapListVariables.TryGetValue(itemID, out GlobalListVariables variable)
                ? variable
                : null;
        }

        public void RequireInitialize()
        {
            if (this.m_MapNameVariables != null && this.m_MapListVariables != null) return;
            
            this.m_MapNameVariables = new Dictionary<IdString, GlobalNameVariables>();
            this.m_MapListVariables = new Dictionary<IdString, GlobalListVariables>();

            foreach (GlobalNameVariables nameVariables in this.m_NameVariables)
            {
                this.m_MapNameVariables[nameVariables.UniqueID] = nameVariables;
            }
            
            foreach (GlobalListVariables listVariables in this.m_ListVariables)
            {
                this.m_MapListVariables[listVariables.UniqueID] = listVariables;
            }
        }
    }
}