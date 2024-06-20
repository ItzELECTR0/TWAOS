using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Serialization.Json
{
    [StructLayout(LayoutKind.Sequential)]
    struct Handle : IEquatable<Handle>
    {
        public static readonly Handle Null = new Handle { Index = -1, Version = -1 };
        
        public int Index;
        public int Version;

        public bool Equals(Handle other)
        {
            return Index == other.Index && Version == other.Version;
        }

        public override bool Equals(object obj)
        {
            return obj is Handle other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Index * 397) ^ Version;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct HandleData
    {
        public int DataIndex;
        public int DataVersion;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct BinaryToken
    {
        public TokenType Type;
        public int HandleIndex;
        public int Position;
        public int Parent;
        public int Length;
        public bool LastCharacterIsEscaped;

        public override string ToString()
        {
            return $"Type=[{Type}] HandleIndex=[{HandleIndex}] Position=[{Position}] Parent=[{Parent}] Length=[{Length}]";
        }
    }

    unsafe struct UnsafePackedBinaryStream : IDisposable
    {
        /// <summary>
        /// All input characters were consumes and all tokens were generated.
        /// </summary>
        public const int ResultSuccess = 0;

        /// <summary>
        /// The maximum depth limit has been exceeded.
        /// </summary>
        public const int ResultStackOverflow = -4;
        
        readonly Allocator m_Label;
        
        public BinaryToken* Tokens;
        public HandleData* Handles;
        public int TokenCapacity;
        public int TokenNextIndex;
        public int TokenParentIndex;

        public byte* Buffer;
        public int BufferCapacity;
        public int BufferPosition;

        public UnsafePackedBinaryStream(int initialTokenCapacity, int initialBufferCapacity, Allocator label)
        {
            m_Label = label;
            
            Tokens = (BinaryToken*) UnsafeUtility.Malloc(sizeof(BinaryToken) * initialTokenCapacity, UnsafeUtility.AlignOf<BinaryToken>(), m_Label);
            Handles = (HandleData*) UnsafeUtility.Malloc(sizeof(HandleData) * initialTokenCapacity, UnsafeUtility.AlignOf<HandleData>(), m_Label);
            TokenCapacity = initialTokenCapacity;
            TokenNextIndex = 0;
            TokenParentIndex = -1;

            Buffer = (byte*) UnsafeUtility.Malloc(sizeof(byte) * initialBufferCapacity, UnsafeUtility.AlignOf<byte>(), m_Label);
            BufferCapacity = initialBufferCapacity;
            BufferPosition = 0;
            
            for (var i = 0; i < initialTokenCapacity; i++)
            {
                Tokens[i] = new BinaryToken
                {
                    HandleIndex = i
                };

                Handles[i] = new HandleData
                {
                    DataIndex = i,
                    DataVersion = 1
                };
            }
        }

        public void Dispose()
        {
            UnsafeUtility.Free(Tokens, m_Label);
            UnsafeUtility.Free(Handles, m_Label);
            UnsafeUtility.Free(Buffer, m_Label);

            Tokens = null;
            Handles = null;
            Buffer = null;
        }

        [BurstDiscard]
        void CheckTokenRangeAndThrow(int index)
        {
            if ((uint) index >= (uint) TokenCapacity)
                throw new IndexOutOfRangeException();
        }
        
        [BurstDiscard]
        void CheckVersionAndThrow(Handle handle, HandleData data)
        {
            if (handle.Version != data.DataVersion)
                throw new InvalidOperationException("View is invalid. The underlying data has been released.");
        }
        
        [BurstDiscard]
        void CheckBufferRangeAndThrow(int length)
        {
            if (length > BufferPosition)
                throw new IndexOutOfRangeException();
        }
        
        internal bool IsValid(Handle handle)
        {
            if ((uint) handle.Index >= (uint) TokenCapacity)
                return false;

            return Handles[handle.Index].DataVersion == handle.Version;
        }

        internal BinaryToken GetToken(int tokenIndex)
        {
            CheckTokenRangeAndThrow(tokenIndex);
            return Tokens[tokenIndex];
        }
        
        internal int GetTokenIndex(Handle handle)
        {
            CheckTokenRangeAndThrow(handle.Index);
            var data = Handles[handle.Index];
            CheckVersionAndThrow(handle, data);
            return data.DataIndex;
        }
        
        internal BinaryToken GetToken(Handle handle)
        {
            CheckTokenRangeAndThrow(handle.Index);
            var data = Handles[handle.Index];
            CheckVersionAndThrow(handle, data);
            return GetToken(data.DataIndex);
        }
        
        internal Handle GetHandle(int tokenIndex)
        {
            CheckTokenRangeAndThrow(tokenIndex);
            var handleData = Handles[Tokens[tokenIndex].HandleIndex];
            return new Handle {Index = Tokens[tokenIndex].HandleIndex, Version = handleData.DataVersion};
        }
        
        internal Handle GetHandle(BinaryToken token)
        {
            return new Handle {Index = token.HandleIndex, Version = Handles[token.HandleIndex].DataVersion };
        }
        
        internal Handle GetFirstChild(Handle handle)
        {
            var start = GetTokenIndex(handle);

            for (var index = start + 1; index < TokenNextIndex; index++)
            {
                var token = GetToken(index);

                if (token.Length != -1 && token.Parent == start && token.Type != TokenType.Comment)
                {
                    return GetHandle(token);
                }
            }

            return Handle.Null;
        }
        
        internal int GetFirstChildIndex(int start)
        {
            if (Tokens[start].Length <= 1)
                return start + 1;
            
            for (var index = start + 1;; index++)
            {
                var token = Tokens[index];

                if (token.Length != -1 && token.Parent == start && token.Type != TokenType.Comment)
                {
                    return index;
                }
            }
        }

        internal T* GetBufferPtr<T>(int tokenIndex) where T : unmanaged
        {
            CheckBufferRangeAndThrow(Tokens[tokenIndex].Position + sizeof(T));
            return (T*)(Buffer + Tokens[tokenIndex].Position);
        }
        
        internal T* GetBufferPtr<T>(Handle handle) where T : unmanaged
        {
            var position = GetToken(handle).Position;
            CheckBufferRangeAndThrow(position + sizeof(T));
            return (T*) (Buffer + position);
        }
        
        internal void EnsureTokenCapacity(int newLength)
        {
            if (newLength <= TokenCapacity)
                return;

            var fromLength = TokenCapacity;

            Tokens = Resize(Tokens, fromLength, newLength, m_Label);
            Handles = Resize(Handles, fromLength, newLength, m_Label);

            for (var index = fromLength; index < newLength; index++)
            {
                Tokens[index] = new BinaryToken
                {
                    HandleIndex = index
                };

                Handles[index] = new HandleData
                {
                    DataIndex = index,
                    DataVersion = 1
                };
            }
            
            TokenCapacity = newLength;
        }
        
        internal void EnsureBufferCapacity(int length)
        {
            if (length <= BufferCapacity)
                return;

            Buffer = Resize(Buffer, BufferPosition, length, m_Label);
            BufferCapacity = length;
        }
        
        public void Clear()
        {
            for (var i=0; i<TokenNextIndex; i++)
                Handles[i].DataVersion++;
            
            TokenNextIndex = 0;
            TokenParentIndex = -1;
            BufferPosition = 0;
        }

        internal int DiscardCompleted()
        {
            const int kStackSize = 128;
            
            var stack = stackalloc int[kStackSize];
            var sp = -1;

            var index = TokenNextIndex - 1;

            while (index != -1 && Tokens[index].Length == -1)
            {
                index = Tokens[index].Parent;
            }

            while (index != -1)
            {
                var token = Tokens[index];
                var partIndex = index + 1;
                var partCount = 1;

                for (;partIndex < TokenNextIndex; partIndex++)
                {
                    if (Tokens[partIndex].Length == -1)
                    {
                        partCount++;
                        continue;
                    }

                    break;
                }

                if (sp + partCount >= kStackSize)
                {
                    return ResultStackOverflow;
                }

                for (var i = partCount - 1; i >= 0; i--)
                {
                    stack[++sp] = index + i;
                }

                index = token.Parent;
            }

            var binaryTokenNextIndex = sp + 1;
            var binaryBufferPosition = 0;

            for (var i = 0; sp >= 0; i++, sp--)
            {
                index = stack[sp];

                if (TokenParentIndex == index)
                {
                    TokenParentIndex = i;
                }

                (Tokens[i], Tokens[index]) = (Tokens[index], Tokens[i]);

                // Update handle pointers
                Handles[Tokens[i].HandleIndex].DataIndex = i;
                Handles[Tokens[index].HandleIndex].DataIndex = index;
                
                var token = Tokens[i];

                var length = 0;

                if (index + 1 >= TokenNextIndex)
                {
                    length = BufferPosition - token.Position;
                }
                else
                {
                    length = Tokens[index + 1].Position - token.Position;
                }

                UnsafeUtility.MemCpy(Buffer + binaryBufferPosition, Buffer + token.Position, length);

                token.Position = binaryBufferPosition;

                var parentIndex = i - 1;
                if (token.Length != -1)
                {
                    while (parentIndex != -1 && Tokens[parentIndex].Length == -1)
                    {
                        parentIndex--;
                    }
                }
                token.Parent = parentIndex;

                Tokens[i] = token;
                binaryBufferPosition += length;
            }

            // Patch the lengths
            for (int i = 0, length = binaryTokenNextIndex; i < binaryTokenNextIndex; i++)
            {
                var token = Tokens[i];

                if (token.Length != -1)
                {
                    token.Length = length;
                }

                length--;
                Tokens[i] = token;
            }

            // Invalidate all views that are outside of the collapsed range
            for (var i = binaryTokenNextIndex; i < TokenNextIndex; i++)
            {
                Handles[Tokens[i].HandleIndex].DataVersion++;
            }

            BufferPosition = binaryBufferPosition;
            TokenNextIndex = binaryTokenNextIndex;
            return ResultSuccess;
        }
        
        static T* Resize<T>(T* buffer, int fromLength, int toLength, Allocator label) where T : unmanaged
        {
            var tmp = (T*) UnsafeUtility.Malloc(toLength * sizeof(T), UnsafeUtility.AlignOf<T>(), label);
            UnsafeUtility.MemCpy(tmp, buffer, fromLength * sizeof(T));
            UnsafeUtility.Free(buffer, label);
            return tmp;
        }
    }

    /// <summary>
    /// Output stream for deserialization. This stream is used to retain relevant data parsed during deserialization.
    /// </summary>
    /// <remarks>
    /// The contents of the stream are not actualized and instead remain as a string of characters.
    /// </remarks>
    public unsafe struct PackedBinaryStream : IDisposable, IEquatable<PackedBinaryStream>
    {
        const int k_DefaultBufferCapacity = 4096;
        const int k_DefaultTokenCapacity = 1024;

        readonly Allocator m_Label;

        [NativeDisableUnsafePtrRestriction] UnsafePackedBinaryStream* m_Data;
        
        internal UnsafePackedBinaryStream* GetUnsafePtr() => m_Data;

        internal int TokenNextIndex => m_Data->TokenNextIndex;

        /// <summary>
        /// Constructs a new instance of <see cref="PackedBinaryStream"/> using default capacities.
        /// </summary>
        /// <param name="label">The memory allocator label to use.</param>
        public PackedBinaryStream(Allocator label) : this(k_DefaultTokenCapacity, k_DefaultBufferCapacity, label)
        {
            
        }

        /// <summary>
        /// Constructs a new instance of <see cref="PackedBinaryStream"/> using the given capacities.
        /// </summary>
        /// <param name="initialTokensCapacity">Initial number of tokens to allocate.</param>
        /// <param name="initialBufferCapacity">Initial buffer size to allocate.</param>
        /// <param name="label">Allocator to use for internal buffers.</param>
        public PackedBinaryStream(int initialTokensCapacity, int initialBufferCapacity, Allocator label)
        {
            m_Label = label;
            m_Data = (UnsafePackedBinaryStream*) UnsafeUtility.Malloc(sizeof(UnsafePackedBinaryStream), UnsafeUtility.AlignOf<UnsafePackedBinaryStream>(), m_Label);
            *m_Data = new UnsafePackedBinaryStream(initialTokensCapacity, initialBufferCapacity, m_Label);
        }
        
        /// <summary>
        /// Releases all resources used by the <see cref="PackedBinaryStream" />.
        /// </summary>
        public void Dispose()
        {
            m_Data->Dispose();
            UnsafeUtility.Free(m_Data, m_Label);
            m_Data = null;
        }

        internal bool IsValid(Handle handle)
        {
            return m_Data->IsValid(handle);
        }
        
        internal BinaryToken GetToken(int tokenIndex)
        {
            return m_Data->GetToken(tokenIndex);
        }

        internal BinaryToken GetToken(Handle handle)
        {
            return m_Data->GetToken(handle);
        }

        internal int GetTokenIndex(Handle handle)
        {
            return m_Data->GetTokenIndex(handle);
        }

        internal Handle GetHandle(int tokenIndex)
        {
            return m_Data->GetHandle(tokenIndex);
        }

        internal Handle GetHandle(BinaryToken token)
        {
            return m_Data->GetHandle(token);
        }

        internal Handle GetFirstChild(Handle handle)
        {
            return m_Data->GetFirstChild(handle);
        }

        internal T* GetBufferPtr<T>(Handle handle) where T : unmanaged
        {
            return m_Data->GetBufferPtr<T>(handle);
        }

        internal void EnsureTokenCapacity(int newLength)
        {
            m_Data->EnsureTokenCapacity(newLength);
        }

        internal void EnsureBufferCapacity(int length)
        {
            m_Data->EnsureBufferCapacity(length);
        }
        
        internal SerializedValueView GetView(int tokenIndex)
        {
            var token = m_Data->Tokens[tokenIndex];
            var handle = m_Data->Handles[token.HandleIndex];
            return new SerializedValueView(m_Data, new Handle { Index = token.HandleIndex, Version = handle.DataVersion });
        }

        /// <summary>
        /// Clears all token and buffer data.
        /// </summary>
        public void Clear()
        {
            m_Data->Clear();
        }

        /// <summary>
        /// Discards completed data from the buffers.
        /// </summary>
        internal void DiscardCompleted()
        {
            m_Data->DiscardCompleted();
        }

        /// <inheritdoc/>
        public bool Equals(PackedBinaryStream other)
        {
            return m_Data == other.m_Data;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is PackedBinaryStream other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) m_Label * 397) ^ unchecked((int) (long) m_Data);
            }
        }
    }
}