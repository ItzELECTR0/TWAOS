using System.IO;

namespace UnityEditor.U2D.Aseprite
{
    /// <summary>
    /// Parsed representation of an Aseprite Old Palette (no. 2) chunk.
    /// </summary>
    /// <note>Not supported yet.</note>
    internal class OldPaletteChunk2 : BaseChunk
    {
        public override ChunkTypes chunkType => ChunkTypes.OldPalette2;

        internal OldPaletteChunk2(uint chunkSize) : base(chunkSize) { }
        protected override void InternalRead(BinaryReader reader) { }
    }
}
