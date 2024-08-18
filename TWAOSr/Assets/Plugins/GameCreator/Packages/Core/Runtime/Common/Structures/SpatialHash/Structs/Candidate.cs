using System;

namespace GameCreator.Runtime.Common
{
    internal readonly struct Candidate : IComparable<Candidate>
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [field: NonSerialized] public int UniqueCode { get; }
        [field: NonSerialized] private float Distance { get; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public Candidate(int uniqueCode, float distance)
        {
            this.UniqueCode = uniqueCode;
            this.Distance = distance;
        }
        
        // EQUALITY: ------------------------------------------------------------------------------

        public int CompareTo(Candidate other)
        {
            return this.Distance.CompareTo(other.Distance);
        }
    }
}