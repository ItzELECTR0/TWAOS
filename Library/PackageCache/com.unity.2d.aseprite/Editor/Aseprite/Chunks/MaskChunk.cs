using System.IO;

namespace UnityEditor.U2D.Aseprite
{
    /// <summary>
    /// Parsed representation of an Aseprite Mask chunk.
    /// </summary>
    /// <note>Not supported yet.</note>
    internal class MaskChunk : BaseChunk
    {
        public override ChunkTypes chunkType => ChunkTypes.Mask;

        internal MaskChunk(uint chunkSize) : base(chunkSize) { }
        protected override void InternalRead(BinaryReader reader) { }
    }
}
