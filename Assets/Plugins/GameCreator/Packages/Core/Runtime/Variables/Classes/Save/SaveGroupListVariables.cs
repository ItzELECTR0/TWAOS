using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    internal class SaveGroupListVariables
    {
        [Serializable]
        private class Group
        {
            [SerializeField] private string m_ID;
            [SerializeField] private SaveSingleListVariables m_Data;

            public string ID => this.m_ID;
            public SaveSingleListVariables Data => this.m_Data;
            
            public Group(string id, ListVariableRuntime runtime)
            {
                this.m_ID = id;
                this.m_Data = new SaveSingleListVariables(runtime);
            }
        }
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private List<Group> m_Groups;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public SaveGroupListVariables(Dictionary<string, ListVariableRuntime> runtime)
        {
            this.m_Groups = new List<Group>();
            
            foreach (KeyValuePair<string, ListVariableRuntime> entry in runtime)
            {
                this.m_Groups.Add(new Group(entry.Key, entry.Value));
            }
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public int Count()
        {
            return m_Groups?.Count ?? 0;
        }

        public string GetID(int index)
        {
            return this.m_Groups?[index].ID ?? string.Empty;
        }
        
        public SaveSingleListVariables GetData(int index)
        {
            return this.m_Groups?[index].Data;
        }
    }
}