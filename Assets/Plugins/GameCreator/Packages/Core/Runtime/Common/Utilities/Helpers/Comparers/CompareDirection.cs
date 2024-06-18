using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class CompareDirection
    {
        private enum Comparison
        {
            Equals,
            AreParallel,
            AreOrthogonal,
            SameDirection,
            EqualMagnitude,
            SmallerMagnitude,
            BiggerMagnitude,
        }
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private Comparison m_Comparison = Comparison.Equals;
        
        [SerializeField] 
        private PropertyGetDirection m_CompareTo = new PropertyGetDirection();
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public CompareDirection()
        { }

        public CompareDirection(PropertyGetDirection direction) : this()
        {
            this.m_CompareTo = direction;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool Match(Vector3 value, Args args)
        {
            Vector3 a = value;
            Vector3 b = this.m_CompareTo.Get(args);

            return this.m_Comparison switch
            {
                Comparison.Equals => a == b,
                Comparison.AreParallel => Mathf.Approximately(Vector3.Dot(a.normalized, b.normalized), 1f),
                Comparison.AreOrthogonal => Mathf.Approximately(Vector3.Dot(a.normalized, b.normalized), 0f),
                Comparison.SameDirection => Vector3.Dot(a.normalized, b.normalized) >= 0f,
                Comparison.EqualMagnitude => Mathf.Approximately(a.magnitude, b.magnitude),
                Comparison.SmallerMagnitude => a.magnitude < b.magnitude,
                Comparison.BiggerMagnitude => a.magnitude > b.magnitude,
                _ => throw new ArgumentOutOfRangeException($"Enum '{this.m_Comparison}' not found")
            };
        }
        
        // STRING: --------------------------------------------------------------------------------

        public override string ToString()
        {
            string operation = this.m_Comparison switch
            {
                Comparison.Equals => "=",
                Comparison.AreParallel => "is parallel with",
                Comparison.AreOrthogonal => "is orthogonal with",
                Comparison.SameDirection => "same direction as",
                Comparison.EqualMagnitude => "same length as",
                Comparison.SmallerMagnitude => "smaller length than",
                Comparison.BiggerMagnitude => "bigger length than",
                _ => string.Empty
            };
            
            return $"{operation} {this.m_CompareTo}";
        }
    }
}