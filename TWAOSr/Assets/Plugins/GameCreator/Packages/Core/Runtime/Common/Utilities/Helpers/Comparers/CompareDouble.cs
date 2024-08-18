using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class CompareDouble
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
        private PropertyGetDecimal m_CompareTo = new PropertyGetDecimal(0f);
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public CompareDouble()
        { }

        public CompareDouble(PropertyGetDecimal number) : this()
        {
            this.m_CompareTo = number;
        }
        
        public CompareDouble(double value) : this(new PropertyGetDecimal(value))
        { }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool Match(double value, Args args)
        {
            double a = value;
            double b = this.m_CompareTo.Get(args);

            return this.m_Comparison switch
            {
                Comparison.Equals => Mathf.Approximately((float) a, (float) b),
                Comparison.Different => !Mathf.Approximately((float) a, (float) b),
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