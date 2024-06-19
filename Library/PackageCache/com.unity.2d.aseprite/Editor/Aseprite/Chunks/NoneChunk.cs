using System.IO;

namespace UnityEditor.U2D.Aseprite
{
    /// <summary>
    /// Empty default chunk.
    /// </summary>
    internal class NoneChunk : BaseChunk
    {
        internal NoneChunk(uint chunkSize) : base(chunkSize) { }
        protected override void InternalRead(BinaryReader reader) { }
    }
}
