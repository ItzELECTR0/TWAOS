using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Serialization.Json.Unsafe;

namespace Unity.Serialization.Json
{
    /// <summary>
    /// A view on top of the <see cref="PackedBinaryStream"/> that represents any value.
    /// </summary>
    public readonly unsafe struct SerializedValueView : ISerializedView
    {
        // ReSharper disable InconsistentNaming
        [NativeDisableUnsafePtrRestriction] internal readonly UnsafePackedBinaryStream* m_Stream;
        internal readonly Handle m_Handle;
        // ReSharper restore InconsistentNaming

        internal SerializedValueView(UnsafePackedBinaryStream* stream, Handle handle)
        {
            m_Stream = stream;
            m_Handle = handle;
        }

        /// <summary>
        /// The <see cref="TokenType"/> for this view. Use this to check which conversions are valid.
        /// </summary>
        public TokenType Type => m_Stream->GetToken(m_Handle).Type;

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="name">The key of the value to get.</param>
        /// <exception cref="InvalidOperationException">The view does not represent an object type.</exception>
        /// <exception cref="KeyNotFoundException">The key does not exist in the collection.</exception>
        [BurstDiscard]
        public SerializedValueView this[string name] => GetValue(name);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="name">The key of the value to get.</param>
        /// <exception cref="InvalidOperationException">The view does not represent an object type.</exception>
        /// <exception cref="KeyNotFoundException">The key does not exist in the collection.</exception>
        public SerializedValueView this[in FixedString32Bytes name] => GetValue(name);
        
        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="name">The key of the value to get.</param>
        /// <exception cref="InvalidOperationException">The view does not represent an object type.</exception>
        /// <exception cref="KeyNotFoundException">The key does not exist in the collection.</exception>
        public SerializedValueView this[in FixedString64Bytes name] => GetValue(name);
        
        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="name">The key of the value to get.</param>
        /// <exception cref="InvalidOperationException">The view does not represent an object type.</exception>
        /// <exception cref="KeyNotFoundException">The key does not exist in the collection.</exception>
        public SerializedValueView this[in FixedString128Bytes name] => GetValue(name);
        
        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="name">The key of the value to get.</param>
        /// <returns>Returns the value associated with the specified key.</returns>
        public SerializedValueView GetValue(in string name)
        {
            if (!TryGetValue(name, out var value))
                throw new KeyNotFoundException(name);
            
            return value;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="name">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value.</param>
        /// <returns>true if the <see cref="SerializedObjectView"/> contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValue(in string name, out SerializedValueView value)
        {
            if (Type == TokenType.Object) 
                return AsObjectView().TryGetValue(name, out value);
            
            value = default;
            return false;
        }
        
        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="name">The key of the value to get.</param>
        /// <typeparam name="T">The fixed string type.</typeparam>
        /// <returns>Returns the value associated with the specified key.</returns>
        public SerializedValueView GetValue<T>(in T name) where T : unmanaged, INativeList<byte>, IUTF8Bytes
        {
            if (!TryGetValue(name, out var value))
                throw new KeyNotFoundException();
            
            return value;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="name">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value.</param>
        /// <typeparam name="T">The fixed string type.</typeparam>
        /// <returns>true if the <see cref="SerializedObjectView"/> contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValue<T>(in T name, out SerializedValueView value) where T : unmanaged, INativeList<byte>, IUTF8Bytes
        {
            if (Type == TokenType.Object) 
                return AsObjectView().TryGetValue(name, out value);
            
            value = default;
            return false;
        }
        
        /// <summary>
        /// Returns true if the value represents a member.
        /// </summary>
        /// <returns>True if the value is a member.</returns>
        public bool IsMember()
        {
            var token = m_Stream->GetToken(m_Handle);

            if (token.Parent != -1 && m_Stream->GetToken(token.Parent).Type != TokenType.Object)
                return false;

            return token.Type == TokenType.String || token.Type == TokenType.Primitive;
        }

        /// <summary>
        /// Returns true if the value represents a null value token.
        /// </summary>
        /// <returns><see langword="true"/> if the value represents a null value token.</returns>
        public bool IsNull()
        {
            var token = m_Stream->GetToken(m_Handle);

            if (token.Type != TokenType.Primitive)
                return false;

            return AsPrimitiveView().IsNull();
        }

        /// <summary>
        /// Reinterprets the value as an array.
        /// </summary>
        /// <returns>The value as a <see cref="SerializedArrayView"/>.</returns>
        public SerializedArrayView AsArrayView()
        {
            CheckValueType(TokenType.Array);
            return new SerializedArrayView(m_Stream, m_Handle);
        }

        /// <summary>
        /// Reinterprets the value as an object.
        /// </summary>
        /// <returns>The value as a <see cref="SerializedObjectView"/>.</returns>
        public SerializedObjectView AsObjectView()
        {
            CheckValueType(TokenType.Object);
            return new SerializedObjectView(m_Stream, m_Handle);
        }

        /// <summary>
        /// Reinterprets the value as an string.
        /// </summary>
        /// <returns>The value as a <see cref="SerializedStringView"/>.</returns>
        /// <exception cref="InvalidOperationException">The value could not be reinterpreted.</exception>
        public SerializedStringView AsStringView()
        {
            var token = m_Stream->GetToken(m_Handle);

            if (token.Type != TokenType.String && token.Type != TokenType.Primitive)
                throw new InvalidOperationException($"Failed to read value RequestedType=[{TokenType.String}|{TokenType.Primitive}] ActualType=[{token.Type}]");

            return new SerializedStringView(m_Stream, m_Handle);
        }

        /// <summary>
        /// Reinterprets the value as a member.
        /// </summary>
        /// <returns>The value as a <see cref="SerializedMemberView"/>.</returns>
        /// <exception cref="InvalidOperationException">The value could not be reinterpreted.</exception>
        public SerializedMemberView AsMemberView()
        {
            if (!IsMember())
            {
                throw new InvalidOperationException($"Failed to read value as member");
            }

            return new SerializedMemberView(m_Stream, m_Handle);
        }

        /// <summary>
        /// Reinterprets the value as a primitive.
        /// </summary>
        /// <returns>The value as a <see cref="SerializedPrimitiveView"/>.</returns>
        public SerializedPrimitiveView AsPrimitiveView()
        {
            CheckValueType(TokenType.Primitive);
            return new SerializedPrimitiveView(m_Stream, m_Handle);
        }

        /// <summary>
        /// Reinterprets the value as a long.
        /// </summary>
        /// <returns>The value as a long.</returns>
        public long AsInt64()
        {
            return AsPrimitiveView().AsInt64();
        }
        
        /// <summary>
        /// Reinterprets the value as a int.
        /// </summary>
        /// <returns>The value as an int.</returns>
        public int AsInt32()
        {
            return (int) AsPrimitiveView().AsInt64();
        }

        /// <summary>
        /// Reinterprets the value as a ulong.
        /// </summary>
        /// <returns>The value as a ulong.</returns>
        public ulong AsUInt64()
        {
            return AsPrimitiveView().AsUInt64();
        }

        /// <summary>
        /// Reinterprets the value as a float.
        /// </summary>
        /// <returns>The value as a float.</returns>
        public float AsFloat()
        {
            return AsPrimitiveView().AsFloat();
        }

        /// <summary>
        /// Reinterprets the value as a double.
        /// </summary>
        /// <returns>The value as a double.</returns>
        public double AsDouble()
        {
            return AsPrimitiveView().AsDouble();
        }
        
        /// <summary>
        /// Reinterprets the value as a bool.
        /// </summary>
        /// <returns>The value as a bool.</returns>
        public bool AsBoolean()
        {
            return AsPrimitiveView().AsBoolean();
        }

        void CheckValueType(TokenType type)
        {
            var token = m_Stream->GetToken(m_Handle);

            if (token.Type != type)
                throw new InvalidOperationException($"Failed to read value RequestedType=[{type}] ActualType=[{token.Type}]");
        }

        /// <summary>
        /// Returns the value as a string.
        /// </summary>
        /// <returns>The value as a string.</returns>
        public override string ToString()
        {
            return AsStringView().ToString();
        }
        
        /// <summary>
        /// Returns the value as a string.
        /// </summary>
        /// <typeparam name="T">The fixed string type.</typeparam>
        /// <returns>The value as a string.</returns>
        public T AsFixedString<T>() where T : unmanaged, INativeList<byte>, IUTF8Bytes
        {
            return AsStringView().AsFixedString<T>();
        }
        
        /// <summary>
        /// Returns the value as a string.
        /// </summary>
        /// <param name="allocator">The allocator to use for the text.</param>
        /// <returns>The value as a string.</returns>
        public NativeText AsNativeText(Allocator allocator)
        {
            return AsStringView().AsNativeText(allocator);
        }
        
        /// <summary>
        /// Returns the value as a string.
        /// </summary>
        /// <param name="allocator">The allocator to use for the text.</param>
        /// <returns>The value as a string.</returns>
        public UnsafeText AsUnsafeText(Allocator allocator)
        {
            return AsStringView().AsUnsafeText(allocator);
        }
        
        internal UnsafeValueView AsUnsafe() => new UnsafeValueView(m_Stream, m_Stream->GetTokenIndex(m_Handle));
    }
}
