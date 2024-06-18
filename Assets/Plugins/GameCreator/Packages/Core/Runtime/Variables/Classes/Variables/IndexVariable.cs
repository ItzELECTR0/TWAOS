using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public class IndexVariable : TVariable
    {
        // PROPERTIES: ----------------------------------------------------------------------------
    
        public override string Title => $"{this.m_Value}";
        
        public override TVariable Copy => new IndexVariable
        {
            m_Value = this.m_Value.Copy
        };
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public IndexVariable() : base()
        { }

        public IndexVariable(IdString typeID) : base(typeID)
        { }
        
        public IndexVariable(TValue value) : this()
        {
            this.m_Value = value;
        }
    }
}