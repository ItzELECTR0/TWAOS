using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    internal class SaveSingleNameVariables
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeReference] private List<NameVariable> m_Variables;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public List<NameVariable> Variables => m_Variables;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public SaveSingleNameVariables(NameVariableRuntime runtime)
        {
            this.m_Variables = new List<NameVariable>();
            foreach (KeyValuePair<string, NameVariable> entry in runtime.Variables)
            {
                this.m_Variables.Add(entry.Value.Copy as NameVariable);
            }
        }
    }
}