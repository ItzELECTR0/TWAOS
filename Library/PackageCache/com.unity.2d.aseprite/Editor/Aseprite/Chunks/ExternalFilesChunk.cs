using System.IO;

namespace UnityEditor.U2D.Aseprite
{
    /// <summary>
    /// Parsed representation of an Aseprite ExternalFiles chunk.
    /// </summary>
    /// <note>Not supported yet.</note>
    internal class ExternalFilesChunk : BaseChunk
    {
        public override ChunkTypes chunkType => ChunkTypes.ExternalFiles;

        internal ExternalFilesChunk(uint chunkSize) : base(chunkSize) { }
        protected override void InternalRead(BinaryReader reader) { }
    }
}
