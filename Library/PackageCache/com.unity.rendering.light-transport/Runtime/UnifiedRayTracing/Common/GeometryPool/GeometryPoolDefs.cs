using System;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
    internal static class GeometryPoolConstants
    {
        public static int GeoPoolPosByteSize = 3 * 4;
        public static int GeoPoolUV0ByteSize = 4 * 4;
        public static int GeoPoolUV1ByteSize = 4 * 4;
        public static int GeoPoolNormalByteSize = 1 * 4;

        public static int GeoPoolPosByteOffset = 0;
        public static int GeoPoolUV0ByteOffset = GeoPoolPosByteOffset + GeoPoolPosByteSize;
        public static int GeoPoolUV1ByteOffset = GeoPoolUV0ByteOffset + GeoPoolUV0ByteSize;
        public static int GeoPoolNormalByteOffset = GeoPoolUV1ByteOffset + GeoPoolUV1ByteSize;

        public static int GeoPoolIndexByteSize = 4;
        public static int GeoPoolVertexByteSize = GeoPoolPosByteSize + GeoPoolUV0ByteSize + GeoPoolUV1ByteSize + GeoPoolNormalByteSize;
    }

    internal struct GeoPoolVertex
    {
        public Vector3 pos;
        public Vector4 uv0;
        public Vector4 uv1;
        public Vector3 N;
    }

    internal struct GeoPoolMeshChunk
    {
        public int indexOffset;
        public int indexCount;
        public int vertexOffset;
        public int vertexCount;
    }

    [Flags]
    internal enum GeoPoolVertexAttribs { Position = 1, Normal = 2, Uv0 = 4, Uv1 = 8 }
}
