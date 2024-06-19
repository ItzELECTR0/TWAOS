using System;
using System.IO;

namespace UnityEditor.U2D.Aseprite
{
    /// <summary>
    /// Flags for layer options.
    /// </summary>
    [Flags]
    public enum LayerFlags
    {
        Visible = 1,
        Editable = 2,
        LockMovement = 4,
        Background = 8,
        PreferLinkedCels = 16,
        DisplayAsCollapsed = 32,
        ReferenceLayer = 64
    }

    /// <summary>
    /// Layer types.
    /// </summary>
    public enum LayerTypes
    {
        Normal = 0,
        Group = 1,
        Tilemap = 2
    }

    /// <summary>
    /// Layer blend modes.
    /// </summary>
    public enum BlendModes
    {
        Normal = 0,
        Multiply = 1,
        Screen = 2,
        Overlay = 3,
        Darken = 4,
        Lighten = 5,
        ColorDodge = 6,
        ColorBurn = 7,
        HardLight = 8,
        SoftLight = 9,
        Difference = 10,
        Exclusion = 11,
        Hue = 12,
        Saturation = 13,
        Color = 14,
        Luminosity = 15,
        Addition = 16,
        Subtract = 17,
        Divide = 18
    }

    /// <summary>
    /// Parsed representation of an Aseprite Layer chunk.
    /// </summary>
    public class LayerChunk : BaseChunk
    {
        public override ChunkTypes chunkType => ChunkTypes.Layer;

        /// <summary>
        /// Layer option flags.
        /// </summary>
        public LayerFlags flags { get; private set; }
        /// <summary>
        /// Type of layer.
        /// </summary>
        public LayerTypes layerType { get; private set; }
        /// <summary>
        /// The child level is used to show the relationship of this layer with the last one read.
        /// </summary>
        public ushort childLevel { get; private set; }
        /// <summary>
        /// Layer blend mode.
        /// </summary>
        public BlendModes blendMode { get; private set; }
        // Layer opacity (0 = transparent, 255 = opaque).
        public byte opacity { get; private set; }
        /// <summary>
        /// Layer name.
        /// </summary>
        public string name { get; private set; }
        /// <summary>
        /// Tileset index (Only available for Tilemap layers).
        /// </summary>
        public uint tileSetIndex { get; private set; }

        internal LayerChunk(uint chunkSize) : base(chunkSize) { }

        protected override void InternalRead(BinaryReader reader)
        {
            flags = (LayerFlags)reader.ReadUInt16();
            layerType = (LayerTypes)reader.ReadUInt16();
            childLevel = reader.ReadUInt16();
            var defaultLayerWidth = reader.ReadUInt16();
            var defaultLayerHeight = reader.ReadUInt16();
            blendMode = (BlendModes)reader.ReadUInt16();
            opacity = reader.ReadByte();

            // Not in use bytes
            for (var i = 0; i < 3; ++i)
                reader.ReadByte();

            name = AsepriteUtilities.ReadString(reader);
            if (layerType == LayerTypes.Tilemap)
                tileSetIndex = reader.ReadUInt32();
        }
    }
}
