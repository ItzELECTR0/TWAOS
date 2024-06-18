using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class ChangeDecimal
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
        [SerializeField] private PropertyGetDecimal m_Value;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public string OperationName => m_Operation.ToString();

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public ChangeDecimal()
        {
            this.m_Operation = Operation.Set;
            this.m_Value = new PropertyGetDecimal();
        }
        
        public ChangeDecimal(double value) : this()
        {
            this.m_Value = new PropertyGetDecimal(value);
        }

        public ChangeDecimal(float value) : this()
        {
            this.m_Value = new PropertyGetDecimal(value);
        }

        public ChangeDecimal(PropertyGetDecimal value)
        {
            this.m_Value = value;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public double Get(double value, Args args)
        {
            return this.m_Operation switch
            {
                Operation.Set => this.m_Value.Get(args),
                Operation.Add => value + this.m_Value.Get(args),
                Operation.Subtract => value - this.m_Value.Get(args),
                Operation.Multiply => value * this.m_Value.Get(args),
                Operation.Divide => value / this.m_Value.Get(args),
                _ => throw new ArgumentOutOfRangeException($"Unknown operation {this.m_Operation}")
            };
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