using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class ChangePosition
    {
        private enum Operation
        {
            Set,
            Add,
            Subtract,
        }
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private Operation m_Operation;
        [SerializeField] private PropertyGetPosition m_Value;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public string OperationName => m_Operation.ToString();

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public ChangePosition()
        {
            this.m_Operation = Operation.Set;
            this.m_Value = new PropertyGetPosition();
        }

        public ChangePosition(Vector3 value) : this()
        {
            this.m_Value = new PropertyGetPosition(value);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Vector3 Get(Vector3 point, Args args)
        {
            Vector3 value = this.m_Value.Get(args);

            return this.m_Operation switch
            {
                Operation.Set => value,
                Operation.Add => point + value,
                Operation.Subtract => point - value,
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
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}