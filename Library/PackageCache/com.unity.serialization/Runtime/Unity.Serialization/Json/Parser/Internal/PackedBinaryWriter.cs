using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Serialization.Json
{
    unsafe struct UnsafePackedBinaryWriter : IDisposable
    {
        readonly UnsafeJsonTokenStream* m_JsonTokenStream;
        readonly UnsafePackedBinaryStream* m_PackedBinaryStream;

        public int JsonTokenNextIndex;
        public int JsonTokenParentIndex;
        
        public bool StripStringEscapeCharacters;
        
        public UnsafePackedBinaryWriter(UnsafeJsonTokenStream* jsonTokenStream, UnsafePackedBinaryStream* packedBinaryStream)
        {
            m_JsonTokenStream = jsonTokenStream;
            m_PackedBinaryStream = packedBinaryStream;
            JsonTokenNextIndex = 0;
            JsonTokenParentIndex = -1;
            StripStringEscapeCharacters = true;
        }
        
        public void Dispose()
        {
        }
        
        public void Seek(int index, int parent)
        {
            JsonTokenNextIndex = index;
            JsonTokenParentIndex = parent;
        }
        
        internal SerializedValueView GetView(int index)
        {
            if ((uint) index >= (uint) m_PackedBinaryStream->TokenNextIndex)
            {
                throw new IndexOutOfRangeException();
            }

            var token = m_PackedBinaryStream->Tokens[index];
            var handle = m_PackedBinaryStream->Handles[token.HandleIndex];
            return new SerializedValueView(m_PackedBinaryStream, new Handle { Index = token.HandleIndex, Version = handle.DataVersion });
        }
        
        public int Write(UnsafeBuffer<char> buffer, int count)
        {
            if (count <= 0)
                return 0;
            
            // NOTE: This method was ported from a parallel job. The previous implementation had a first pass 'WriteTokens'
            //       which computes the start and end points for characters we care about in the input buffer. The 'WriteCharacters`
            //       step would then go wide and copy all characters to the final binary buffer. 
            // 
            // TODO: This method should be re-worked with raw single method performance in mind.
            
            var length = Math.Min(JsonTokenNextIndex + count, m_JsonTokenStream->TokenNextIndex);
            count = length - JsonTokenNextIndex;

            var inputTokenStartIndex = JsonTokenNextIndex;
            var outputTokenStartIndex = m_PackedBinaryStream->TokenNextIndex;

            m_PackedBinaryStream->EnsureTokenCapacity(m_PackedBinaryStream->TokenNextIndex + count);

            var binaryBufferPosition = WriteTokens(buffer, count);

            m_PackedBinaryStream->EnsureBufferCapacity(binaryBufferPosition);

            WriteCharacters(inputTokenStartIndex, outputTokenStartIndex, buffer, count);

            m_PackedBinaryStream->BufferPosition = binaryBufferPosition;

            return count;
        }
        
        int WriteTokens(UnsafeBuffer<char> buffer, int count)
        {
            var binaryBufferPosition = m_PackedBinaryStream->BufferPosition;
            var inputTokenLength = Math.Min(JsonTokenNextIndex + count, m_JsonTokenStream->TokenNextIndex);
            
            for (; JsonTokenNextIndex < inputTokenLength; JsonTokenNextIndex++, m_PackedBinaryStream->TokenNextIndex++)
            {
                var inputToken = m_JsonTokenStream->Tokens[JsonTokenNextIndex];

                while (JsonTokenParentIndex != -1 && inputToken.Parent < JsonTokenParentIndex)
                {
                    var inputTokenParent = m_JsonTokenStream->Tokens[JsonTokenParentIndex];

                    JsonTokenParentIndex = inputTokenParent.Parent;

                    if (m_PackedBinaryStream->TokenParentIndex == -1)
                    {
                        continue;
                    }

                    m_PackedBinaryStream->Tokens[m_PackedBinaryStream->TokenParentIndex].Length = inputTokenParent.Start == -1 ? -1 : m_PackedBinaryStream->TokenNextIndex - m_PackedBinaryStream->TokenParentIndex;
                    m_PackedBinaryStream->TokenParentIndex = m_PackedBinaryStream->Tokens[m_PackedBinaryStream->TokenParentIndex].Parent;
                }

                var binaryToken = m_PackedBinaryStream->Tokens[m_PackedBinaryStream->TokenNextIndex];
                binaryToken.Type = inputToken.Type;
                binaryToken.Position = binaryBufferPosition;
                binaryToken.Parent = m_PackedBinaryStream->TokenParentIndex;
                binaryToken.Length = inputToken.Start == -1 ? -1 : 1;
                binaryToken.LastCharacterIsEscaped = false;

                switch (inputToken.Type)
                {
                    case TokenType.Array:
                    case TokenType.Object:
                    {
                        JsonTokenParentIndex = JsonTokenNextIndex;
                        m_PackedBinaryStream->TokenParentIndex = m_PackedBinaryStream->TokenNextIndex;
                    }
                    break;

                    case TokenType.Primitive:
                    case TokenType.String:
                    case TokenType.Comment:
                    {
                        if (inputToken.Parent == -1 || m_JsonTokenStream->Tokens[inputToken.Parent].Type == TokenType.Object || inputToken.End == -1)
                        {
                            JsonTokenParentIndex = JsonTokenNextIndex;
                            m_PackedBinaryStream->TokenParentIndex = m_PackedBinaryStream->TokenNextIndex;
                        }

                        if (inputToken.Start != -1)
                        {
                            binaryBufferPosition += sizeof(int);
                        }
                        
                        var start = inputToken.Start != -1 ? inputToken.Start : 0;
                        var end = inputToken.End != -1 ? inputToken.End : buffer.Length;

                        if (StripStringEscapeCharacters && inputToken.Type == TokenType.String)
                        {
                            var validCharacterCount = 0;
                            
                            for (var i = start; i < end; i++)
                            {
                                if (buffer.Buffer[i] == '\\')
                                {
                                    i++;

                                    if (i < end)
                                    {
                                        validCharacterCount++;
                                    }
                                    else 
                                    {
                                        // Our last character was an escaped character. We need to record this so the next token write can swap the character.
                                        binaryToken.LastCharacterIsEscaped = true;
                                    }
                                }
                                else
                                {
                                    validCharacterCount++;
                                }
                            }
                            
                            binaryBufferPosition += validCharacterCount * sizeof(ushort);
                        }
                        else
                        {
                            binaryBufferPosition += (end - start) * sizeof(ushort);
                        }
                    }
                    break;
                }
                
                m_PackedBinaryStream->Tokens[m_PackedBinaryStream->TokenNextIndex] = binaryToken;
                m_PackedBinaryStream->Handles[binaryToken.HandleIndex].DataVersion++;
            }

            // Patch up the lengths
            for (int inputTokenIndex = JsonTokenNextIndex - 1, outputTokenIndex = m_PackedBinaryStream->TokenNextIndex - 1; inputTokenIndex >= 0 && outputTokenIndex >= 0;)
            {
                var inputToken = m_JsonTokenStream->Tokens[inputTokenIndex];
                var binaryToken = m_PackedBinaryStream->Tokens[outputTokenIndex];

                m_PackedBinaryStream->Tokens[outputTokenIndex].Length = inputToken.Start == -1 ? -1 : m_PackedBinaryStream->TokenNextIndex - outputTokenIndex;

                inputTokenIndex = inputToken.Parent;
                outputTokenIndex = binaryToken.Parent;
            }

            if (JsonTokenNextIndex >= m_JsonTokenStream->TokenNextIndex)
            {
                if (!IsObjectKey(m_JsonTokenStream->Tokens, JsonTokenNextIndex - 1))
                {
                    // Close the stack
                    while (JsonTokenParentIndex >= 0)
                    {
                        var index = JsonTokenParentIndex;
                        var token = m_JsonTokenStream->Tokens[index];

                        if (token.End == -1)
                        {
                            break;
                        }

                        JsonTokenParentIndex = token.Parent;

                        if (m_PackedBinaryStream->TokenParentIndex == -1)
                        {
                            continue;
                        }

                        m_PackedBinaryStream->TokenParentIndex = m_PackedBinaryStream->Tokens[m_PackedBinaryStream->TokenParentIndex].Parent;
                    }
                }
            }

            return binaryBufferPosition;
        }

        void WriteCharacters(int inputTokenStartIndex, int outputTokenStartIndex, UnsafeBuffer<char> buffer, int count)
        {
            for (var index=0; index<count; index++)
            {
                var inputTokenIndex = index + inputTokenStartIndex;
                var inputToken = m_JsonTokenStream->Tokens[inputTokenIndex];
                
                switch (inputToken.Type)
                {
                    case TokenType.String:
                    case TokenType.Primitive:
                    case TokenType.Comment:
                    {
                        var binaryTokenIndex = index + outputTokenStartIndex;
                        var binaryToken = m_PackedBinaryStream->Tokens[binaryTokenIndex];
                        var position = binaryToken.Position;
                        
                        if (inputToken.Start != -1)
                        {
                            position += sizeof(int);
                        }

                        var start = inputToken.Start != -1 ? inputToken.Start : 0;
                        var end = inputToken.End != -1 ? inputToken.End : buffer.Length;

                        if (inputToken.Type == TokenType.String && StripStringEscapeCharacters)
                        {
                            void WriteCharacterEscaped(ref UnsafePackedBinaryWriter writer, ref int p, char c)
                            {
                                switch (c)
                                {
                                    case '\\':
                                        writer.Write(ref p, '\\');
                                        break;
                                    case '"':
                                        writer.Write(ref p, '\"');
                                        break;
                                    case 't':
                                        writer.Write(ref p, '\t');
                                        break;
                                    case 'r':
                                        writer.Write(ref p, '\r');
                                        break;
                                    case 'n':
                                        writer.Write(ref p, '\n');
                                        break;
                                    case 'b':
                                        writer.Write(ref p, '\b');
                                        break;
                                    case '0':
                                        writer.Write(ref p, '\0');
                                        break;
                                    default:
                                        writer.Write(ref p, '\0');
                                        break;
                                }
                            }

                            var i = start;

                            if (i < end && inputToken.Start == -1 && m_PackedBinaryStream->Tokens[binaryTokenIndex - 1].LastCharacterIsEscaped)
                                WriteCharacterEscaped(ref this, ref position, buffer.Buffer[i++]);
                            
                            for (; i < end; i++)
                            {
                                if (buffer.Buffer[i] == '\\')
                                {
                                    i++;

                                    if (i < end)
                                        WriteCharacterEscaped(ref this, ref position, buffer.Buffer[i]);
                                }
                                else
                                {
                                    Write(ref position, buffer.Buffer[i]);
                                }
                            }
                        }
                        else
                        {
                            for (var i = start; i < end; i++)
                            {
                                Write(ref position, ((ushort*) buffer.Buffer)[i]);
                            }
                        }

                        if (inputToken.End != -1)
                        {
                            var startTokenIndex = inputTokenIndex;

                            for (;;)
                            {
                                if (inputToken.Start != -1 || inputToken.Parent == -1)
                                {
                                    break;
                                }

                                startTokenIndex = inputToken.Parent;
                                inputToken = m_JsonTokenStream->Tokens[startTokenIndex];
                            }

                            var offset = startTokenIndex - inputTokenIndex;
                            var startPosition = m_PackedBinaryStream->Tokens[binaryTokenIndex + offset].Position;
                            var byteLength = position - startPosition;
                            byteLength -= sizeof(int);
                            Write(ref startPosition, byteLength / sizeof(ushort));
                        }
                    }
                    break;
                }
            }
        }

        void Write<T>(ref int position, T value) where T : unmanaged
        {
            *(T*) (m_PackedBinaryStream->Buffer + position) = value;
            position += sizeof(T);
        }

        /// <summary>
        /// Returns true if the given token is an object key.
        /// </summary>
        static bool IsObjectKey(Token* tokens, int index)
        {
            var token = tokens[index];

            if (token.Type != TokenType.String && token.Type != TokenType.Primitive)
            {
                return false;
            }

            if (token.Parent == -1)
            {
                return false;
            }

            var parent = tokens[token.Parent];

            return parent.Type == TokenType.Object;
        }
    }
    
    unsafe struct PackedBinaryWriter : IDisposable
    {
        readonly Allocator m_Label;
        [NativeDisableUnsafePtrRestriction] UnsafePackedBinaryWriter* m_Data;

        public int TokenNextIndex => m_Data->JsonTokenNextIndex;
        public int TokenParentIndex => m_Data->JsonTokenParentIndex;
        
        /// <summary>
        /// Indicates if escape characters should be stripped during reads.
        /// </summary>
        public bool StripStringEscapeCharacters
        {
            get => m_Data->StripStringEscapeCharacters;
            set => m_Data->StripStringEscapeCharacters = value;
        }

        public PackedBinaryWriter(JsonTokenStream tokenStream, PackedBinaryStream binaryStream, Allocator label)
        {
            m_Label = label;
            m_Data = (UnsafePackedBinaryWriter*) UnsafeUtility.Malloc(sizeof(UnsafePackedBinaryWriter), UnsafeUtility.AlignOf<UnsafePackedBinaryWriter>(), label);
            *m_Data = new UnsafePackedBinaryWriter(tokenStream.GetUnsafePtr(), binaryStream.GetUnsafePtr());
        }

        public void Dispose()
        {
            m_Data->Dispose();
            UnsafeUtility.Free(m_Data, m_Label);
            m_Data = null;
        }

        /// <summary>
        /// Seeks the writer to the given token/parent combination.
        /// </summary>
        public void Seek(int index, int parent)
        {
            m_Data->Seek(index, parent);
        }

        internal SerializedValueView GetView(int index)
        {
            return m_Data->GetView(index);
        }

        /// <summary>
        /// Writes tokens and characters to the internal binary stream.
        /// </summary>
        /// <param name="buffer">A character array containing the input data that was tokenized.</param>
        /// <param name="count">The number of tokens to write.</param>
        /// <returns>The number of tokens written.</returns>
        public int Write(UnsafeBuffer<char> buffer, int count)
        {
            return m_Data->Write(buffer, count);
        }
    }
}
