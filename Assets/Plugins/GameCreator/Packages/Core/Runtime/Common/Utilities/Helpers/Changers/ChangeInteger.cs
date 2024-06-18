using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class ChangeInteger
    {
        private enum Operation
        {
            Set,
            Add,
            Subtract,
            Multiply,
            Divide,
        }
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private Operation m_Operation;
        [SerializeField] private PropertyGetInteger m_Value;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public string OperationName => m_Operation.ToString();

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public ChangeInteger()
        {
            this.m_Operation = Operation.Set;
            this.m_Value = new PropertyGetInteger();
        }

        public ChangeInteger(int value) : this()
        {
            this.m_Value = new PropertyGetInteger(value);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public int Get(int value, Args args)
        {
            double result = this.m_Operation switch
            {
                Operation.Set => this.m_Value.Get(args),
                Operation.Add => value + this.m_Value.Get(args),
                Operation.Subtract => value - this.m_Value.Get(args),
                Operation.Multiply => value * this.m_Value.Get(args),
                Operation.Divide => value / this.m_Value.Get(args),
                _ => throw new ArgumentOutOfRangeException($"Unknown operation {this.m_Operation}")
            };

            return (int) result;
        }
        
        // OVERRIDES: -----------------------------------------------------------------------------

        public override string ToString()
        {
            return this.m_Operation switch
            {
                Operation.Set => $"= {this.m_Value}",
                Operation.Add => $"+ {this.m_Value}",
                Operation.Subtract => $"- {this.m_Value}",
                Operation.Multiply => $"* {this.m_Value}",
                Operation.Divide => $"/ {this.m_Value}",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}