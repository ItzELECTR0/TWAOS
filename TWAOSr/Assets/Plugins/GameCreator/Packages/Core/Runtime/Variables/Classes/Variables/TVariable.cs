using System;
using System.Reflection;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public abstract class TVariable : TPolymorphicItem<TVariable>
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeReference] protected TValue m_Value = new ValueNull();
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public object Value
        {
            get => this.m_Value?.Value;
            set
            {
                if (this.m_Value == null) return;
                this.m_Value.Value = value;
            }
        }

        public IdString TypeID => this.m_Value.TypeID;
        public Type Type => this.m_Value.Type;

        public Texture Icon => this.m_Value.GetType().GetCustomAttribute<ImageAttribute>()?.Image;

        public abstract override string Title { get; }

        public abstract TVariable Copy { get; }
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        protected TVariable()
        { }

        protected TVariable(IdString typeID)
        {
            this.m_Value = TValue.CreateValue(typeID);
        }
    }
}