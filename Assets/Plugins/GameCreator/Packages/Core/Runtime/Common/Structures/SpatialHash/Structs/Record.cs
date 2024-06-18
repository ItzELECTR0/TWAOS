using Unity.Mathematics;

namespace GameCreator.Runtime.Common
{
    internal struct Record
    {
        public int UniqueCode { get; }
        public float3 Position { get; }
        public bool IsDynamic { get; }

        public Record(int uniqueCode, float3 position, bool isDynamic)
        {
            this.UniqueCode = uniqueCode;
            this.Position = position;
            this.IsDynamic = isDynamic;
        }
    }
}