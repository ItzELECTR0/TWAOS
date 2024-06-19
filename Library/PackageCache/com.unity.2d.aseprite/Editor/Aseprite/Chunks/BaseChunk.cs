using System;
using System.IO;
using UnityEngine;

namespace UnityEditor.U2D.Aseprite
{
    /// <summary>
    /// Aseprite Chunk Types.
    /// </summary>
    public enum ChunkTypes
    {
        None = 0,
        OldPalette = 0x0004,
        OldPalette2 = 0x0011,
        Layer = 0x2004,
        Cell = 0x2005,
        CellExtra = 0x2006,
        ColorProfile = 0x2007,
        ExternalFiles = 0x2008,
        Mask = 0x2016,
        Path = 0x2017,
        Tags = 0x2018,
        Palette = 0x2019,
        UserData = 0x2020,
        Slice = 0x2022,
        Tileset = 0x2023
    }

    /// <summary>
    /// The header of each chunk.
    /// </summary>
    public class ChunkHeader
    {
        /// <summary>
        /// The stride of the chunk header in bytes.
        /// </summary>
        public const int stride = 6;
        /// <summary>
        /// The size of the chunk in bytes.
        /// </summary>
        public uint chunkSize { get; private set; }
        /// <summary>
        /// The type of the chunk.
        /// </summary>
        public ChunkTypes chunkType { get; private set; }

        internal void Read(BinaryReader reader)
        {
            chunkSize = reader.ReadUInt32();
            chunkType = (ChunkTypes)reader.ReadUInt16();
        }
    }

    public abstract class BaseChunk : IDisposable
    {
        /// <summary>
        /// The type of the chunk.
        /// </summary>
        public virtual ChunkTypes chunkType => ChunkTypes.None;

        protected readonly uint m_ChunkSize;

        protected BaseChunk(uint chunkSize)
        {
            m_ChunkSize = chunkSize;
        }

        internal bool Read(BinaryReader reader)
        {
            var bytes = reader.ReadBytes((int)m_ChunkSize - ChunkHeader.stride);
            using var memoryStream = new MemoryStream(bytes);
            using var chunkReader = new BinaryReader(memoryStream);

            try
            {
                InternalRead(chunkReader);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to read a chunk of type: {chunkType}. Skipping the chunk. \nException: {e}");
                return false;
            }

            return true;
        }

        protected abstract void InternalRead(BinaryReader reader);

        public virtual void Dispose() { }
    }
}
