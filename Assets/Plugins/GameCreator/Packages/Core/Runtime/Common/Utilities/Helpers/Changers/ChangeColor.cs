using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class ChangeColor
    {
        private enum Operation
        {
            Set,
            Add,
            Subtract,
            Multiply
        }
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private Operation m_Operation;
        [SerializeField] private PropertyGetColor m_Value;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public string OperationName => m_Operation.ToString();

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public ChangeColor()
        {
            this.m_Operation = Operation.Set;
            this.m_Value = new PropertyGetColor();
        }

        public ChangeColor(Color value) : this()
        {
            this.m_Value = new PropertyGetColor(value);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Color Get(Color value, Args args)
        {
            return this.m_Operation switch
            {
                Operation.Set => this.m_Value.Get(args),
                Operation.Add => value + this.m_Value.Get(args),
                Operation.Subtract => value - this.m_Value.Get(args),
                Operation.Multiply => value * this.m_Value.Get(args),
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