using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public class FieldGetLocalList : TFieldGetVariable
    {
        [SerializeField]
        protected PropertyGetGameObject m_Variable = new PropertyGetGameObject();

        [SerializeReference]
        protected TListGetPick m_Select = new GetPickFirst();

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public FieldGetLocalList(IdString typeID)
        {
            this.m_TypeID = typeID;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override object Get(Args args)
        {
            LocalListVariables instance = this.m_Variable.Get<LocalListVariables>(args); 
            return instance != null ? instance.Get(this.m_Select, args) : null;
        }

        public override string ToString() => this.m_Variable != null
            ? $"{m_Variable}[{this.m_Select}]"
            : "(none)";
    }
}