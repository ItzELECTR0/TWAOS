using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Serialization.Json.Unsafe
{
    /// <summary>
    /// A view on top of the <see cref="PackedBinaryStream"/> that represents a string.
    /// </summary>
    public readonly unsafe struct UnsafeStringView : IEquatable<string>
    {
        [NativeDisableUnsafePtrRestriction] readonly UnsafePackedBinaryStream* m_Stream;
        readonly int m_TokenIndex;
        
        internal UnsafeStringView(UnsafePackedBinaryStream* stream, int tokenIndex)
        {
            m_Stream = stream;
            m_TokenIndex = tokenIndex;
        }

        /// <summary>
        /// Gets the number of characters in the <see cref="SerializedStringView"/>.
        /// </summary>
        /// <returns>The number of characters in the string.</returns>
        public unsafe int Length()
        {
            return *m_Stream->GetBufferPtr<int>(m_TokenIndex);
        }

        /// <summary>
        /// Gets the <see cref="char"/> at a specified position in the current <see cref="SerializedStringView"/>.
        /// </summary>
        /// <param name="index">A position in the current string.</param>
        /// <exception cref="IndexOutOfRangeException"><see cref="index"/> is greater than or equal to the length of this object or less than zero.</exception>
        public unsafe char this[int index]
        {
            get
            {
                var ptr = m_Stream->GetBufferPtr<byte>(m_TokenIndex);

                if ((uint) index > *(int*) ptr)
                {
                    throw new IndexOutOfRangeException();
                }

                var chars = (char*) (ptr + sizeof(int));
                return chars[index];
            }
        }

        /// <summary>
        /// Determines whether this view and another specified <see cref="string"/> object have the same value.
        /// </summary>
        /// <param name="other">The string to compare to this view.</param>
        /// <returns>true if the value of the value parameter is the same as the value of this view; otherwise, false.</returns>
        public unsafe bool Equals(string other)
        {
            var ptr = m_Stream->GetBufferPtr<byte>(m_TokenIndex);

            if (null == other)
            {
                return *(int*) ptr == 0;
            }

            if (other.Length != *(int*) ptr)
            {
                return false;
            }

            var chars = (char*) (ptr + sizeof(int));

            for (var i = 0; i < other.Length; i++)
            {
                if (chars[i] != other[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Allocates and returns a new string instance based on the view.
        /// </summary>
        /// <returns>A new <see cref="string"/> instance.</returns>
        public override unsafe string ToString()
        {
            var buffer = m_Stream->GetBufferPtr<byte>(m_TokenIndex);
            var len = *(int*) buffer;
            var ptr = (char*) (buffer + sizeof(int));
            return new string(ptr, 0, len);
        }
        
        public SerializedStringView AsSafe() => new SerializedStringView(m_Stream, m_Stream->GetHandle(m_TokenIndex));
    }
}