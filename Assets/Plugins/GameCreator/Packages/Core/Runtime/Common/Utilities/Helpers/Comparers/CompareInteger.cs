using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class CompareInteger
    {
        private enum Comparison
        {
            Equals,
            Different,
            Less,
            Greater,
            LessOrEqual,
            GreaterOrEqual
        }
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private Comparison m_Comparison = Comparison.Equals;
        
        [SerializeField] 
        private PropertyGetInteger m_CompareTo = new PropertyGetInteger(0);
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public CompareInteger()
        { }
        
        public CompareInteger(int value) : this(GetDecimalInteger.Create(value))
        { }

        public CompareInteger(PropertyGetInteger number) : this()
        {
            this.m_CompareTo = number;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool Match(int value, Args args)
        {
            int a = value;
            int b = (int) this.m_CompareTo.Get(args);

            return this.m_Comparison switch
            {
                Comparison.Equals => a == b,
                Comparison.Different => a != b,
                Comparison.Less => a < b,
                Comparison.Greater => a > b,
                Comparison.LessOrEqual => a <= b,
                Comparison.GreaterOrEqual => a >= b,
                _ => throw new ArgumentOutOfRangeException($"Enum '{this.m_Comparison}' not found")
            };
        }
        
        // STRING: --------------------------------------------------------------------------------

        public override string ToString()
        {
            string operation = this.m_Comparison switch
            {
                Comparison.Equals => "=",
                Comparison.Different => "≠",
                Comparison.Less => "<",
                Comparison.Greater => ">",
                Comparison.LessOrEqual => "≤",
                Comparison.GreaterOrEqual => "≥",
                _ => string.Empty
            };
            
            return $"{operation} {this.m_CompareTo}";
        }
    }
}