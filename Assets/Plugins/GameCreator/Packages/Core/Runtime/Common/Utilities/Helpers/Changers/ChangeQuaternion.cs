using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class ChangeQuaternion
    {
        private enum Operation
        {
            Set,
            Add,
            Subtract
        }
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private Operation m_Operation;
        [SerializeField] private PropertyGetRotation m_Value;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public string OperationName => m_Operation.ToString();

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public ChangeQuaternion()
        {
            this.m_Operation = Operation.Set;
            this.m_Value = new PropertyGetRotation();
        }

        public ChangeQuaternion(Quaternion value) : this()
        {
            this.m_Value = new PropertyGetRotation(value);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Quaternion Get(Quaternion rotation, Args args)
        {
            return this.m_Operation switch
            {
                Operation.Set => this.m_Value.Get(args),
                Operation.Add => rotation * this.m_Value.Get(args),
                Operation.Subtract => rotation * Quaternion.Inverse(this.m_Value.Get(args)),
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