using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Serialization.Json.Unsafe
{
    readonly unsafe struct UnsafePrimitiveView
    {
        [NativeDisableUnsafePtrRestriction] readonly UnsafePackedBinaryStream* m_Stream;
        readonly int m_TokenIndex;

        internal UnsafePrimitiveView(UnsafePackedBinaryStream* stream, int tokenIndex)
        {
            m_Stream = stream;
            m_TokenIndex = tokenIndex;
        }

        /// <summary>
        /// Returns a string view over the primitive.
        /// </summary>
        /// <returns>A <see cref="SerializedStringView"/> over this primitive.</returns>
        public UnsafeStringView AsStringView() => new UnsafeStringView(m_Stream, m_TokenIndex);

        /// <summary>
        /// Returns true if the primitive represents infinity.
        /// </summary>
        /// <returns>True if this primitive is infinity; false otherwise.</returns>
        public bool IsInfinity()
        {
            var ptr = m_Stream->GetBufferPtr<byte>(m_TokenIndex);
            var len = *(int*) ptr;
            var chars = (char*) (ptr + sizeof(int));
            if (Convert.IsSigned(chars, len))
            {
                chars++;
                len--;
            }
            return Convert.MatchesInfinity(chars, len);
        }

        /// <summary>
        /// Returns true if the primitive represents a value that is not a number.
        /// </summary>
        /// <returns>True if this primitive is nan; false otherwise.</returns>
        public bool IsNaN()
        {
            var ptr = m_Stream->GetBufferPtr<byte>(m_TokenIndex);
            var len = *(int*) ptr;
            var chars = (char*) (ptr + sizeof(int));
            return Convert.MatchesNaN(chars, len);
        }

        /// <summary>
        /// Returns true if the primitive represents a value that is null.
        /// </summary>
        /// <returns>True if this primitive is null; false otherwise.</returns>
        public bool IsNull()
        {
            var ptr = m_Stream->GetBufferPtr<byte>(m_TokenIndex);
            var len = *(int*) ptr;
            var chars = (char*) (ptr + sizeof(int));
            return Convert.MatchesNull(chars, len);
        }

        /// <summary>
        /// Returns true if the primitive is an integral type.
        /// </summary>
        /// <returns>True if this primitive is an integral type; false otherwise.</returns>
        public bool IsIntegral()
        {
            var ptr = m_Stream->GetBufferPtr<byte>(m_TokenIndex);
            return Convert.IsIntegral((char*) (ptr + sizeof(int)), *(int*) ptr);
        }

        /// <summary>
        /// Returns true if the primitive is a decimal type.
        /// </summary>
        /// <returns>True if this primitive is an decimal type; false otherwise.</returns>
        public bool IsDecimal()
        {
            var ptr = m_Stream->GetBufferPtr<byte>(m_TokenIndex);
            return Convert.IsDecimal((char*) (ptr + sizeof(int)), *(int*) ptr);
        }

        /// <summary>
        /// Returns true if the primitive is a signed type.
        /// </summary>
        /// <returns>True if this primitive is an signed type; false otherwise.</returns>
        public bool IsSigned()
        {
            var ptr = m_Stream->GetBufferPtr<byte>(m_TokenIndex);
            return Convert.IsSigned((char*) (ptr + sizeof(int)), *(int*) ptr);
        }

        /// <summary>
        /// Returns true if the primitive is a boolean type.
        /// </summary>
        /// <returns>True if this primitive is an boolean type; false otherwise.</returns>
        public bool IsBoolean()
        {
            var ptr = m_Stream->GetBufferPtr<byte>(m_TokenIndex);
            var length = *(int*) ptr;
            var chars = (char*) (ptr + sizeof(int));
            return Convert.MatchesTrue(chars, length) || Convert.MatchesFalse(chars, length);
        }

        /// <summary>
        /// Reinterprets the primitive as a long.
        /// </summary>
        /// <returns>The primitive as a long.</returns>
        /// <exception cref="ParseErrorException">The parser failed to convert the characters.</exception>
        public long AsInt64()
        {
            var ptr = m_Stream->GetBufferPtr<byte>(m_TokenIndex);
            var result = Convert.StrToInt64((char*) (ptr + sizeof(int)), *(int*) ptr, out var value);
            if (result != Convert.ParseError.None)
            {
                throw new ParseErrorException($"Failed to parse Value=[{AsString()}] as Type=[{typeof(long)}] ParseError=[{result}]");
            }
            return value;
        }

        /// <summary>
        /// Reinterprets the primitive as a ulong.
        /// </summary>
        /// <returns>The primitive as a ulong.</returns>
        /// <exception cref="ParseErrorException">The parser failed to convert the characters.</exception>
        public ulong AsUInt64()
        {
            var ptr = m_Stream->GetBufferPtr<byte>(m_TokenIndex);
            var result = Convert.StrToUInt64((char*) (ptr + sizeof(int)), *(int*) ptr, out var value);
            if (result != Convert.ParseError.None)
            {
                throw new ParseErrorException($"Failed to parse Value=[{AsString()}] as Type=[{typeof(ulong)}] ParseError=[{result}]");
            }
            return value;
        }

        /// <summary>
        /// Reinterprets the primitive as a float.
        /// </summary>
        /// <returns>The primitive as a float.</returns>
        /// <exception cref="ParseErrorException">The parser failed to convert the characters.</exception>
        public float AsFloat()
        {
            var ptr = m_Stream->GetBufferPtr<byte>(m_TokenIndex);
            var result = Convert.StrToFloat32((char*) (ptr + sizeof(int)), *(int*) ptr, out var value);
            if (result != Convert.ParseError.None)
            {
                throw new ParseErrorException($"Failed to parse Value=[{AsString()}] as Type=[{typeof(float)}] ParseError=[{result}]");
            }
            return value;
        }

        /// <summary>
        /// Reinterprets the primitive as a double.
        /// </summary>
        /// <remarks>
        /// This method relies on a string allocation for <see cref="double.Parse(string)"/>.
        /// </remarks>
        /// <returns>The primitive as a double.</returns>
        public double AsDouble()
        {
            return double.Parse(AsString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Reinterprets the primitive as a bool.
        /// </summary>
        /// <returns>The primitive as a bool.</returns>
        /// <exception cref="ParseErrorException">The parser failed to convert the characters.</exception>
        public bool AsBoolean()
        {
            var ptr = m_Stream->GetBufferPtr<byte>(m_TokenIndex);
            var length = *(int*) ptr;
            var chars = (char*) (ptr + sizeof(int));

            if (Convert.MatchesTrue(chars, length))
            {
                return true;
            }

            if (Convert.MatchesFalse(chars, length))
            {
                return false;
            }

            throw new ParseErrorException($"Failed to parse Value=[{AsString()}] as Type=[{typeof(bool)}]");
        }

        /// <summary>
        /// Allocates and returns a new <see cref="string"/> for the primitive.
        /// </summary>
        /// <returns>A <see cref="string"/> copy of the primitive.</returns>
        public string AsString() => AsStringView().ToString();
        
        public SerializedPrimitiveView AsSafe() => new SerializedPrimitiveView(m_Stream, m_Stream->GetHandle(m_TokenIndex));
    }
}
