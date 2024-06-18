using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public class FieldSetLocalList : TFieldSetVariable
    {
        [SerializeField]
        protected PropertyGetGameObject m_Variable = new PropertyGetGameObject();

        [SerializeReference]
        protected TListSetPick m_Select = new SetPickFirst();

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public FieldSetLocalList(IdString typeID)
        {
            this.m_TypeID = typeID;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override void Set(object value, Args args)
        {
            LocalListVariables instance = this.m_Variable.Get<LocalListVariables>(args);
            if (instance != null) instance.Set(this.m_Select, value, args);
        }

        public override object Get(Args args)
        {
            LocalListVariables instance = this.m_Variable.Get<LocalListVariables>(args);
            return instance != null ? instance.Get(this.m_Select, args) : null;
        }

        public override string ToString() => $"{this.m_Variable}[{this.m_Select}]";
    }
}