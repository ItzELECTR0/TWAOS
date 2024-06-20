using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Serialization.Json.Unsafe;

namespace Unity.Serialization.Json
{
    /// <summary>
    /// A view on top of the <see cref="PackedBinaryStream"/> that represents a string.
    /// </summary>
    public readonly unsafe struct SerializedStringView : ISerializedView, IEquatable<string>
    {
        [NativeDisableUnsafePtrRestriction] readonly UnsafePackedBinaryStream* m_Stream;
        readonly Handle m_Handle;

        internal SerializedStringView(UnsafePackedBinaryStream* stream, Handle handle)
        {
            m_Stream = stream;
            m_Handle = handle;
        }

        /// <summary>
        /// Gets the number of characters in the <see cref="SerializedStringView"/>.
        /// </summary>
        /// <returns>The number of characters in the string.</returns>
        public int Length()
        {
            return *m_Stream->GetBufferPtr<int>(m_Handle);
        }

        /// <summary>
        /// Gets the <see cref="char"/> at a specified position in the current <see cref="SerializedStringView"/>.
        /// </summary>
        /// <param name="index">A position in the current string.</param>
        /// <exception cref="IndexOutOfRangeException"><see cref="index"/> is greater than or equal to the length of this object or less than zero.</exception>
        public char this[int index]
        {
            get
            {
                var ptr = m_Stream->GetBufferPtr<byte>(m_Handle);

                if ((uint) index > *(int*) ptr)
                    throw new IndexOutOfRangeException();

                var chars = (char*) (ptr + sizeof(int));
                return chars[index];
            }
        }

        /// <summary>
        /// Determines whether this view and another specified <see cref="string"/> object have the same value.
        /// </summary>
        /// <param name="other">The string to compare to this view.</param>
        /// <returns>true if the value of the value parameter is the same as the value of this view; otherwise, false.</returns>
        public bool Equals(string other)
        {
            var ptr = m_Stream->GetBufferPtr<byte>(m_Handle);

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
        /// Determines whether this view and another specified <see cref="string"/> object have the same value.
        /// </summary>
        /// <param name="other">The string to compare to this view.</param>
        /// <typeparam name="T">The fixed string type.</typeparam>
        /// <returns>true if the value of the value parameter is the same as the value of this view; otherwise, false.</returns>
        public bool Equals<T>(T other) where T : unmanaged, INativeList<byte>, IUTF8Bytes
        {
            var buffer = m_Stream->GetBufferPtr<byte>(m_Handle);

            var length = *(int*)buffer;
            var chars = (char*) (buffer + sizeof(int));

            return UTF8ArrayUnsafeUtility.StrCmp(other.GetUnsafePtr(), other.Length, chars, length) == 0;
        }
        
        /// <summary>
        /// Allocates and returns a new string instance based on the view.
        /// </summary>
        /// <returns>A new <see cref="string"/> instance.</returns>
        public override string ToString()
        {
            var buffer = m_Stream->GetBufferPtr<byte>(m_Handle);
            var ptr = (char*) (buffer + sizeof(int));
            var len = *(int*) buffer;

            return new string(ptr, 0, len);
        }

        /// <summary>
        /// Allocates and returns a new FixedString instance based on the view.
        /// </summary>
        /// <typeparam name="T">The fixed string type.</typeparam>
        /// <returns>A new <see cref="FixedString"/> instance.</returns>
        public T AsFixedString<T>() where T : unmanaged, INativeList<byte>, IUTF8Bytes
        {
            var buffer = m_Stream->GetBufferPtr<byte>(m_Handle);
            var ptr = (char*) (buffer + sizeof(int));
            var len = *(int*) buffer;

            var str = new T();
            var error = Unicode.Utf16ToUtf8(ptr, len, str.GetUnsafePtr(), out var utf8Length, str.Capacity);
            
            if (error != ConversionError.None)
                throw new Exception("ConversionError");
            
            str.Length = utf8Length;
            return str;
        }
        
        /// <summary>
        /// Allocates and returns a new <see cref="NativeText"/> instance based on the view.
        /// </summary>
        /// <param name="allocator">The allocator to use for the text.</param>
        /// <returns>A new <see cref="NativeText"/> instance.</returns>
        public NativeText AsNativeText(Allocator allocator)
        {
            var buffer = m_Stream->GetBufferPtr<byte>(m_Handle);
            var ptr = (char*) (buffer + sizeof(int));
            var len = *(int*) buffer;

            var text = new NativeText(len, allocator);
            var error = Unicode.Utf16ToUtf8(ptr, len, text.GetUnsafePtr(), out var utf8Length, len);
            text.Length = utf8Length;
            
            if (error != ConversionError.None)
                throw new Exception("ConversionError");
            
            return text;
        }
        
        /// <summary>
        /// Allocates and returns a new <see cref="UnsafeText"/> instance based on the view.
        /// </summary>
        /// <param name="allocator">The allocator to use for the text.</param>
        /// <returns>A new <see cref="UnsafeText"/> instance.</returns>
        public UnsafeText AsUnsafeText(Allocator allocator)
        {
            var buffer = m_Stream->GetBufferPtr<byte>(m_Handle);
            var ptr = (char*) (buffer + sizeof(int));
            var len = *(int*) buffer;

            var text = new UnsafeText(len, allocator);
            var error = Unicode.Utf16ToUtf8(ptr, len, text.GetUnsafePtr(), out var utf8Length, len);
            text.Length = utf8Length;
            
            if (error != ConversionError.None)
                throw new Exception("ConversionError");
            
            return text;
        }

        internal UnsafeStringView AsUnsafe() => new UnsafeStringView(m_Stream, m_Stream->GetTokenIndex(m_Handle));
    }
}
