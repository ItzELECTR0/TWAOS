using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    internal class SaveGroupNameVariables
    {
        [Serializable]
        private class Group
        {
            [SerializeField] private string m_ID;
            [SerializeField] private SaveSingleNameVariables m_Data;

            public string ID => this.m_ID;
            public SaveSingleNameVariables Data => this.m_Data;
            
            public Group(string id, NameVariableRuntime runtime)
            {
                this.m_ID = id;
                this.m_Data = new SaveSingleNameVariables(runtime);
            }
        }
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private List<Group> m_Groups;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public SaveGroupNameVariables(Dictionary<string, NameVariableRuntime> runtime)
        {
            this.m_Groups = new List<Group>();
            
            foreach (KeyValuePair<string, NameVariableRuntime> entry in runtime)
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
        
        public SaveSingleNameVariables GetData(int index)
        {
            return this.m_Groups?[index].Data;
        }
    }
}