using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public class IndexList : TList<IndexVariable>
    {
        // MEMBERS: -------------------------------------------------------------------------------
    
        [SerializeField] private IdString m_TypeID = ValueNull.TYPE_ID;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public IdString TypeID => this.m_TypeID;

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public IndexList() : base()
        { }
        
        public IndexList(IdString typeID) : this()
        { 
            this.m_TypeID = typeID;
        }

        public IndexList(IdString typeID, params IndexVariable[] variables) : base(variables)
        {
            this.m_TypeID = typeID;
        }
    }
}