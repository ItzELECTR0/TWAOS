using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class ChangeDirection
    {
        private enum Operation
        {
            Set,
            Add,
            Subtract,
            Cross,
            Project,
            Max,
            Min
        }
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private Operation m_Operation;
        [SerializeField] private PropertyGetDirection m_Value;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public string OperationName => m_Operation.ToString();

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public ChangeDirection()
        {
            this.m_Operation = Operation.Set;
            this.m_Value = new PropertyGetDirection();
        }

        public ChangeDirection(Vector3 value) : this()
        {
            this.m_Value = new PropertyGetDirection(value);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Vector3 Get(Vector3 value, Args args)
        {
            return this.m_Operation switch
            {
                Operation.Set => this.m_Value.Get(args),
                Operation.Add => value + this.m_Value.Get(args),
                Operation.Subtract => value - this.m_Value.Get(args),
                Operation.Cross => Vector3.Cross(value, this.m_Value.Get(args)),
                Operation.Project => Vector3.Project(value, this.m_Value.Get(args)),
                Operation.Max => Vector3.Max(value, this.m_Value.Get(args)),
                Operation.Min => Vector3.Min(value, this.m_Value.Get(args)),
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
                _ => $"{this.m_Operation} {this.m_Value}"
            };
        }
    }
}