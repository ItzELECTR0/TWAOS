using System;
using System.Collections.ObjectModel;
using System.IO;

namespace UnityEditor.U2D.Aseprite
{
    /// <summary>
    /// Flags to define where data for a tileset is stored.
    /// </summary>
    [Flags]
    public enum TileSetFlags
    {
        IncludesLinkToExternal = 1,
        IncludesTilesInFile = 2,
        Misc = 4,
    }

    /// <summary>
    /// Parsed representation of an Aseprite Tileset chunk.
    /// </summary>
    /// <note>Not supported yet.</note>
    public class TilesetChunk : BaseChunk
    {
        public override ChunkTypes chunkType => ChunkTypes.Tileset;

        /// <summary>
        /// The ID of the tileset.
        /// </summary>
        public uint tileSetId { get; private set; }
        /// <summary>
        /// Flags to define where data for a tileset is stored.
        /// </summary>
        public TileSetFlags tileSetFlags { get; private set; }
        /// <summary>
        /// The number of tiles in the tileset.
        /// </summary>
        public uint noOfTiles { get; private set; }
        /// <summary>
        /// Tile width in pixels.
        /// </summary>
        public ushort width { get; private set; }
        /// <summary>
        /// Tile height in pixels.
        /// </summary>
        public ushort height { get; private set; }
        /// <summary>
        /// The name of the tileset.
        /// </summary>
        public string tileSetName { get; private set; }

        readonly ushort m_ColorDepth;
        readonly ReadOnlyCollection<PaletteEntry> m_PaletteEntries;
        readonly byte m_AlphaPaletteEntry;

        internal TilesetChunk(uint chunkSize, ushort colorDepth, ReadOnlyCollection<PaletteEntry> paletteEntries, byte alphaPaletteEntry) : base(chunkSize)
        {
            m_ColorDepth = colorDepth;
            m_PaletteEntries = paletteEntries;
            m_AlphaPaletteEntry = alphaPaletteEntry;
        }

        protected override void InternalRead(BinaryReader reader)
        {
            tileSetId = reader.ReadUInt32();
            tileSetFlags = (TileSetFlags)reader.ReadUInt32();
            noOfTiles = reader.ReadUInt32();
            width = reader.ReadUInt16();
            height = reader.ReadUInt16();

            var baseIndex = reader.ReadInt16();
            var reservedBytes = reader.ReadBytes(14);

            tileSetName = AsepriteUtilities.ReadString(reader);

            // Not supported yet.
            if ((tileSetFlags & TileSetFlags.IncludesLinkToExternal) != 0)
            {
                var idOfExternalFile = reader.ReadUInt32();
                var tileSetIdInExternal = reader.ReadUInt32();
            }
            if ((tileSetFlags & TileSetFlags.IncludesTilesInFile) != 0)
            {
                var compressedDataLength = (int)reader.ReadUInt32();
                var decompressedData = AsepriteUtilities.ReadAndDecompressedData(reader, compressedDataLength);

                var image = AsepriteUtilities.GenerateImageData(m_ColorDepth, decompressedData, m_PaletteEntries, m_AlphaPaletteEntry);

                // Disposing for now.
                image.Dispose();
            }
        }
    }
}
