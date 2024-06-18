using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public class FieldSetGlobalName : TFieldSetVariable
    {
        [SerializeField] protected GlobalNameVariables m_Variable;
        [SerializeField] protected IdPathString m_Name;

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public FieldSetGlobalName(IdString typeID)
        {
            this.m_TypeID = typeID;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override void Set(object value, Args args)
        {
            if (this.m_Variable == null) return;
            this.m_Variable.Set(this.m_Name.String, value);
        }
        
        public override object Get(Args args)
        {
            return this.m_Variable != null ? this.m_Variable.Get(this.m_Name.String) : null;
        }

        public override string ToString()
        {
            return string.Format(
                "{0}{1}",
                this.m_Variable != null ? m_Variable.name : "(none)",
                string.IsNullOrEmpty(this.m_Name.String) ? string.Empty : $"[{this.m_Name.String}]" 
            );
        }
    }
}