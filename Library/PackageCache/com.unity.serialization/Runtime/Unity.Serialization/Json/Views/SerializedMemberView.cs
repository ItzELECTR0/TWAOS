using Unity.Collections.LowLevel.Unsafe;
using Unity.Serialization.Json.Unsafe;

namespace Unity.Serialization.Json
{
    /// <summary>
    /// A view on top of the <see cref="PackedBinaryStream"/> that represents a key-value pair.
    /// </summary>
    public readonly unsafe struct SerializedMemberView
    {
        [NativeDisableUnsafePtrRestriction] readonly UnsafePackedBinaryStream* m_Stream;
        readonly Handle m_Handle;

        internal SerializedMemberView(UnsafePackedBinaryStream* stream, Handle handle)
        {
            m_Stream = stream;
            m_Handle = handle;
        }
        
        /// <summary>
        /// Returns a <see cref="SerializedStringView"/> over the name of this member.
        /// </summary>
        /// <returns>A view over the name.</returns>
        public SerializedStringView Name() => new SerializedStringView(m_Stream, m_Handle);

        /// <summary>
        /// Returns a <see cref="SerializedValueView"/> over the value of this member.
        /// </summary>
        /// <returns>A view over the value.</returns>
        public SerializedValueView Value() => new SerializedValueView(m_Stream, m_Stream->GetFirstChild(m_Handle));
        
        internal UnsafeMemberView AsUnsafe() => new UnsafeMemberView(m_Stream, m_Stream->GetTokenIndex(m_Handle));
    }
}