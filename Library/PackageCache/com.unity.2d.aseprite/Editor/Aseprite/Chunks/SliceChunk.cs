using System.IO;

namespace UnityEditor.U2D.Aseprite
{
    /// <summary>
    /// Parsed representation of an Aseprite Slice chunk.
    /// </summary>
    /// <note>Not supported yet.</note>
    internal class SliceChunk : BaseChunk
    {
        public override ChunkTypes chunkType => ChunkTypes.Slice;

        internal SliceChunk(uint chunkSize) : base(chunkSize) { }
        protected override void InternalRead(BinaryReader reader) { }
    }
}
