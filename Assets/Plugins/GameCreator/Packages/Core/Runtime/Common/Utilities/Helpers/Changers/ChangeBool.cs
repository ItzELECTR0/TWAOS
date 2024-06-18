using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class ChangeBool
    {
        private enum Operation
        {
            Set,
            OR,
            AND,
            XOR,
            NOR,
            NAND,
            NXOR
        }
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private Operation m_Operation;
        [SerializeField] private PropertyGetBool m_Value;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public string OperationName => m_Operation.ToString();

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public ChangeBool()
        {
            this.m_Operation = Operation.Set;
            this.m_Value = new PropertyGetBool();
        }

        public ChangeBool(bool value) : this()
        {
            this.m_Value = new PropertyGetBool(value);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool Get(bool value, Args args)
        {
            return this.m_Operation switch
            {
                Operation.Set => this.m_Value.Get(args),
                Operation.OR => value || this.m_Value.Get(args),
                Operation.AND => value && this.m_Value.Get(args),
                Operation.XOR => value != this.m_Value.Get(args),
                Operation.NOR => !(value || this.m_Value.Get(args)),
                Operation.NAND => !(value && this.m_Value.Get(args)),
                Operation.NXOR => value == this.m_Value.Get(args),
                _ => throw new ArgumentOutOfRangeException($"Unknown operation {this.m_Operation}")
            };
        }
        
        // OVERRIDES: -----------------------------------------------------------------------------

        public override string ToString()
        {
            return $"{this.m_Operation} {this.m_Value}";
        }
    }
}