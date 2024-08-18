using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public class NameVariable : TVariable
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private IdString m_Name;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public string Name => this.m_Name.String;

        public override string Title => $"{this.m_Name.String}: {this.m_Value}";

        public override TVariable Copy => new NameVariable
        {
            m_Name = this.m_Name,
            m_Value = this.m_Value.Copy
        };

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public NameVariable() : base()
        { }
        
        public NameVariable(IdString typeID) : base(typeID)
        { }

        public NameVariable(string name, TValue value) : this()
        {
            this.m_Name = new IdString(name);
            this.m_Value = value;
        }
    }
}