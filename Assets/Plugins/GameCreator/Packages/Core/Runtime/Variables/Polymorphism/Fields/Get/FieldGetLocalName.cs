using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public class FieldGetLocalName : TFieldGetVariable
    {
        [SerializeReference]
        protected PropertyGetGameObject m_Variable = new PropertyGetGameObject();
        
        [SerializeField] protected IdPathString m_Name;

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public FieldGetLocalName(IdString typeID)
        {
            this.m_TypeID = typeID;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public override object Get(Args args)
        {
            LocalNameVariables instance = this.m_Variable.Get<LocalNameVariables>(args);
            return instance != null ? instance.Get(m_Name.String) : null;
        }

        public override string ToString()
        {
            return string.Format(
                "{0}{1}",
                this.m_Variable,
                string.IsNullOrEmpty(this.m_Name.String) ? string.Empty : $"[{this.m_Name.String}]" 
            );
        }
    }
}