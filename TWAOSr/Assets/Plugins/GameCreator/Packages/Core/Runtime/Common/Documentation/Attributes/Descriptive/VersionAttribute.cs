using System;

namespace GameCreator.Runtime.Common
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct,
        Inherited = false
    )]
    public class VersionAttribute : Attribute
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }

        public VersionAttribute(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public override string ToString()
        {
            return $"{this.X}.{this.Y}.{this.Z}";
        }

        public bool EqualTo(VersionAttribute other)
        {
            return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
        }

        public int CompareTo(VersionAttribute other)
        {
            if (this.X > other.X) return 1;
            if (this.X < other.X) return -1;
            
            if (this.Y > other.Y) return 1;
            if (this.Y < other.Y) return -1;
            
            if (this.Z > other.Z) return 1;
            if (this.Z < other.Z) return -1;
            
            return 0;
        }
    }
}
