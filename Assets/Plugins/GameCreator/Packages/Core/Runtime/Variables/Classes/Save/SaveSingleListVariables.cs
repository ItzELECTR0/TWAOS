using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    internal class SaveSingleListVariables
    {
        [SerializeField] private IdString m_TypeID;
        [SerializeReference] private List<IndexVariable> m_Variables;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public IdString TypeID => m_TypeID;

        public List<IndexVariable> Variables => m_Variables;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public SaveSingleListVariables(ListVariableRuntime runtime)
        {
            this.m_TypeID = runtime.TypeID;
            this.m_Variables = new List<IndexVariable>();

            for (int i = 0; i < runtime.Count; ++i)
            {
                this.m_Variables.Add(runtime.Variables[i].Copy as IndexVariable);
            }
        }
    }
}