using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Serialization.Json
{
    /// <summary>
    /// Internal struct used to track the type of comment being parsed.
    /// </summary>
    enum JsonCommentType
    {
        /// <summary>
        /// The comment type is not yet known. E.g. we have only encountered the first `/`.
        /// </summary>
        Unknown,
            
        /// <summary>
        /// Single line comment prefixed with `//` and ending in `\n`.
        /// </summary>
        SingleLine,
            
        /// <summary>
        /// Multi line comment prefixed with `/*` and ending with `*/`.
        /// </summary>
        MultiLine
    }
    
    unsafe struct UnsafeJsonTokenizer : IDisposable
    {
        public ushort PrevChar;
        public int ResultCode;

        UnsafeJsonTokenStream* m_TokenStream;
        JsonCommentType m_CommentType;
        ushort* m_CharBuffer;
        int m_CharBufferLength;
        int m_CharBufferPosition;
        bool m_IsEnd;
        
        /// <summary>
        /// Track if the previous character is escaped. This needs to be stored to handle re-entry in streamed scenarios.
        /// </summary>
        bool m_NextStringCharIsEscaped;

        public UnsafeJsonTokenizer(Allocator allocator)
        {
            PrevChar = 0;
            ResultCode = 0;
            
            m_TokenStream = null;
            m_CommentType = JsonCommentType.Unknown;
            m_CharBuffer = null;
            m_CharBufferLength = 0;
            m_CharBufferPosition = 0;
            m_IsEnd = false;
            m_NextStringCharIsEscaped = false;
        }

        public void Dispose()
        {
        }

        public void Reset()
        {
            PrevChar = 0;
            ResultCode = 0;
            
            m_TokenStream = null;
            m_CommentType = JsonCommentType.Unknown;
            m_CharBuffer = null;
            m_CharBufferLength = 0;
            m_CharBufferPosition = 0;
            m_IsEnd = false;
            m_NextStringCharIsEscaped = false;
        }

        /// <summary>
        /// Writes <see cref="T:Unity.Serialization.Token" /> objects to the internal buffer.
        /// </summary>s
        /// <param name="stream">The output stream to write to.</param>
        /// <param name="charBuffer">A character array containing the input json data to tokenize.</param>
        /// <param name="charBufferStart">The index of ptr at which to begin reading.</param>
        /// <param name="charBufferLength">The maximum number of characters to read.</param>
        /// <param name="end">A value indicating if this is the final block of characters from a stream. This will trigger an error for any unclosed scopes.</param>
        /// <returns>The number of characters that have been read.</returns>
        public int Write(UnsafeJsonTokenStream* stream, ushort* charBuffer, int charBufferStart, int charBufferLength, bool end)
        {
            m_TokenStream = stream;
            m_CharBuffer = charBuffer;
            m_CharBufferPosition = charBufferStart;
            m_CharBufferLength = charBufferLength;
            m_IsEnd = end;
            
            if (m_TokenStream->TokenNextIndex > 0 && m_TokenStream->Tokens[m_TokenStream->TokenNextIndex - 1].End == -1)
            {
                var token = m_TokenStream->Tokens[m_TokenStream->TokenNextIndex - 1];

                switch (token.Type)
                {
                    case TokenType.String:
                    {
                        var result = ParseString(m_TokenStream->TokenNextIndex - 1, UnsafeJsonTokenStream.PartialTokenStart);

                        if (result != JsonTokenizer.ResultSuccess)
                        {
                            ResultCode = result;
                            return m_CharBufferPosition - charBufferStart;
                        }

                        m_CharBufferPosition++;
                    }
                    break;
                    
                    case TokenType.Primitive:
                    {
                        var result = ParsePrimitive(m_TokenStream->TokenNextIndex - 1, UnsafeJsonTokenStream.PartialTokenStart);

                        if (result != JsonTokenizer.ResultSuccess)
                        {
                            ResultCode = result;
                            return m_CharBufferPosition - charBufferStart;
                        }

                        m_CharBufferPosition++;
                    }
                    break;
                    
                    case TokenType.Comment:
                    {
                        var result = ParseComment(m_TokenStream->TokenNextIndex - 1, UnsafeJsonTokenStream.PartialTokenStart);

                        if (result != JsonTokenizer.ResultSuccess)
                        {
                            ResultCode = result;
                            return m_CharBufferPosition - charBufferStart;
                        }

                        m_CharBufferPosition++;
                    }
                        break;
                }
            }

            for (; m_CharBufferPosition < m_CharBufferLength; m_CharBufferPosition++)
            {
                var c = m_CharBuffer[m_CharBufferPosition];

                switch (c)
                {
                    case '{':
                    case '[':
                    {
                        m_TokenStream->Add(new Token
                        {
                            Type = c == '{' ? TokenType.Object : TokenType.Array,
                            Parent = m_TokenStream->TokenParentIndex,
                            Start = m_CharBufferPosition,
                            End = -1
                        });

                        m_TokenStream->TokenParentIndex = m_TokenStream->TokenNextIndex - 1;
                    }
                    break;

                    case '}':
                    case ']':
                    {
                        var type = c == '}' ? TokenType.Object : TokenType.Array;

                        if (m_TokenStream->TokenNextIndex <= 0)
                        {
                            ResultCode = JsonTokenizer.ResultInvalidInput;
                            return m_CharBufferPosition - charBufferStart;
                        }

                        var index = m_TokenStream->TokenNextIndex - 1;

                        for (;;)
                        {
                            var token = m_TokenStream->Tokens[index];

                            if (token.Start != UnsafeJsonTokenStream.PartialTokenStart && token.End == UnsafeJsonTokenStream.PartialTokenEnd && token.Type != TokenType.String && token.Type != TokenType.Primitive)
                            {
                                if (token.Type != type)
                                {
                                    ResultCode = JsonTokenizer.ResultInvalidInput;
                                    return m_CharBufferPosition - charBufferStart;
                                }

                                m_TokenStream->TokenParentIndex = token.Parent;
                                token.End = m_CharBufferPosition + 1;
                                m_TokenStream->Tokens[index] = token;
                                break;
                            }

                            if (token.Parent == -1)
                            {
                                if (token.Type != type || m_TokenStream->TokenParentIndex == -1)
                                {
                                    ResultCode = JsonTokenizer.ResultInvalidInput;
                                    return m_CharBufferPosition - charBufferStart;
                                }

                                break;
                            }

                            index = token.Parent;
                        }

                        var parent = m_TokenStream->TokenParentIndex != -1 ? m_TokenStream->Tokens[m_TokenStream->TokenParentIndex] : default;

                        if (m_TokenStream->TokenParentIndex != -1 &&
                            parent.Type != TokenType.Object &&
                            parent.Type != TokenType.Array)
                        {
                            m_TokenStream->TokenParentIndex = m_TokenStream->Tokens[m_TokenStream->TokenParentIndex].Parent;
                        }
                    }
                        break;

                    case '/':
                    {
                        m_CharBufferPosition++;
                        
                        PrevChar = 0;
                        m_CommentType = JsonCommentType.Unknown;
                            
                        var result = ParseComment(m_TokenStream->TokenParentIndex, m_CharBufferPosition + 1);
                        
                        if (result == JsonTokenizer.ResultInvalidInput)
                        {
                            ResultCode = JsonTokenizer.ResultInvalidInput;
                            return m_CharBufferPosition - charBufferStart;
                        }
                    }
                        break;
                    
                    case '\t':
                    case '\r':
                    case ' ':
                    case '\n':
                    case '\0':
                    case ':':
                    case '=':
                    case ',':
                    {
                    }
                        break;

                    default:
                    {
                        int result;

                        if (c == '"')
                        {
                            m_CharBufferPosition++;

                            PrevChar = 0;
                            
                            result = ParseString(m_TokenStream->TokenParentIndex, m_CharBufferPosition);
                            
                            if (result == JsonTokenizer.ResultInvalidInput)
                            {
                                ResultCode = JsonTokenizer.ResultInvalidInput;
                                return m_CharBufferPosition - charBufferStart;
                            }
                        }
                        else
                        {
                            var start = m_CharBufferPosition;

                            result = ParsePrimitive(m_TokenStream->TokenParentIndex, start);

                            if (result == JsonTokenizer.ResultInvalidInput)
                            {
                                ResultCode = JsonTokenizer.ResultInvalidInput;
                                return m_CharBufferPosition - charBufferStart;
                            }
                        }

                        if (m_TokenStream->TokenParentIndex == -1 || m_TokenStream->Tokens[m_TokenStream->TokenParentIndex].Type == TokenType.Object)
                        {
                            m_TokenStream->TokenParentIndex = m_TokenStream->TokenNextIndex - 1;
                        }
                        else if (m_TokenStream->TokenParentIndex != -1 &&
                                 m_TokenStream->Tokens[m_TokenStream->TokenParentIndex].Type != TokenType.Object &&
                                 m_TokenStream->Tokens[m_TokenStream->TokenParentIndex].Type != TokenType.Array)
                        {
                            m_TokenStream->TokenParentIndex = m_TokenStream->Tokens[m_TokenStream->TokenParentIndex].Parent;
                        }

                        if (result == JsonTokenizer.ResultEndOfStream)
                        {
                            ResultCode = JsonTokenizer.ResultEndOfStream;
                            return m_CharBufferPosition - charBufferStart;
                        }
                    }
                    break;
                }
            }

            ResultCode = JsonTokenizer.ResultSuccess;
            return m_CharBufferPosition - charBufferStart;
        }
        
        int ParseString(int parent, int start)
        {
            for (; m_CharBufferPosition < m_CharBufferLength; m_CharBufferPosition++)
            {
                var c = m_CharBuffer[m_CharBufferPosition];
                
                if (c == '\\')
                {
                    m_NextStringCharIsEscaped = !m_NextStringCharIsEscaped;
                }
                else if (c == '"' && !m_NextStringCharIsEscaped)
                {
                    m_NextStringCharIsEscaped = false;
                    
                    m_TokenStream->Add(new Token
                    {
                        Type = TokenType.String,
                        Parent = parent,
                        Start = start,
                        End = m_CharBufferPosition
                    });

                    break;
                }
                else
                {
                    m_NextStringCharIsEscaped = false;
                }

                PrevChar = c;
            }

            if (m_CharBufferPosition >= m_CharBufferLength)
            {
                m_TokenStream->Add(new Token
                {
                    Type = TokenType.String,
                    Parent = parent,
                    Start = start,
                    End = m_IsEnd ? m_CharBufferPosition : -1
                });

                return JsonTokenizer.ResultEndOfStream;
            }

            return JsonTokenizer.ResultSuccess;
        }

        int ParsePrimitive(int parent, int start)
        {
            for (; m_CharBufferPosition < m_CharBufferLength; m_CharBufferPosition++)
            {
                var c = m_CharBuffer[m_CharBufferPosition];

                if (c == ' ' ||
                    c == '\t' ||
                    c == '\r' ||
                    c == '\n' ||
                    c == '\0' ||
                    c == ',' ||
                    c == ']' ||
                    c == '}' ||
                    c == ':' ||
                    c == '=')
                {
                    m_TokenStream->Add(new Token
                    {
                        Type = TokenType.Primitive,
                        Parent = parent,
                        Start = start,
                        End = m_CharBufferPosition
                    });

                    m_CharBufferPosition--;
                    break;
                }

                if (c < 32 || c >= 127)
                    return JsonTokenizer.ResultInvalidInput;
            }

            if (m_CharBufferPosition >= m_CharBufferLength)
            {
                m_TokenStream->Add(new Token
                {
                    Type = TokenType.Primitive,
                    Parent = parent,
                    Start = start,
                    End = m_IsEnd ? m_CharBufferPosition : -1
                });

                return JsonTokenizer.ResultEndOfStream;
            }

            return JsonTokenizer.ResultSuccess;
        }

        int ParseComment(int parent, int start)
        {
            for (; m_CharBufferPosition < m_CharBufferLength; m_CharBufferPosition++)
            {
                var c = m_CharBuffer[m_CharBufferPosition];

                switch (m_CommentType)
                {
                    case JsonCommentType.Unknown:
                    {
                        switch ((char) c)
                        {
                            case '/':
                                m_CommentType = JsonCommentType.SingleLine;
                                continue;
                            case '*':
                                m_CommentType = JsonCommentType.MultiLine;
                                continue;
                            default:
                                return JsonTokenizer.ResultInvalidInput;
                        }
                    }

                    case JsonCommentType.SingleLine:
                    {
                        switch ((char) c)
                        {
                            case '\n':
                            case '\0':
                            {
                                m_TokenStream->Add(new Token
                                {
                                    Type = TokenType.Comment,
                                    Parent = parent,
                                    Start = start,
                                    End = m_CharBufferPosition - 1
                                });

                                return JsonTokenizer.ResultSuccess;
                            }
                        }
                    }
                        break;

                    case JsonCommentType.MultiLine:
                    {
                        if (c == '/' && PrevChar == '*')
                        {
                            m_TokenStream->Add(new Token
                            {
                                Type = TokenType.Comment,
                                Parent = parent,
                                Start = start,
                                End = m_CharBufferPosition - 1
                            });
                            
                            return JsonTokenizer.ResultSuccess;
                        }
                    }
                        break;
                }

                PrevChar = c;
            }
            
            if (m_CharBufferPosition >= m_CharBufferLength)
            {
                m_TokenStream->Add(new Token
                {
                    Type = TokenType.Comment,
                    Parent = parent,
                    Start = start,
                    End = m_IsEnd ? m_CharBufferPosition : -1
                });
            }
            
            return JsonTokenizer.ResultEndOfStream;
        }
    }
    
    /// <summary>
    /// The tokenizer is the lowest level API for json deserialization.
    ///
    /// It's only job is to parse characters into an array of <see cref="Token"/> simple structs.
    ///
    /// e.g. {"foo": 10} becomes
    ///
    ///  [0] Type=[JsonType.Object]    Range=[0..11] Parent=[-1]
    ///  [1] Type=[JsonType.String]    Range=[2..5]  Parent=[0]
    ///  [2] Type=[JsonType.Primitive] Range=[8..10] Parent=[1]
    ///
    /// @NOTE No conversion or copying of data takes place here.
    ///
    /// Implementation based off of https://github.com/zserge/jsmn
    /// </summary>
    unsafe struct JsonTokenizer : IDisposable
    {
        /// <summary>
        /// All input characters were consumed and all tokens were generated.
        /// </summary>
        internal const int ResultSuccess = 0;

        /// <summary>
        /// The input data was invalid in some way.
        /// </summary>
        internal const int ResultInvalidInput = -1;

        /// <summary>
        /// All input characters were consumed and the writer is expecting more
        /// </summary>
        internal const int ResultEndOfStream = -2;

        readonly Allocator m_Label;
        [NativeDisableUnsafePtrRestriction] UnsafeJsonTokenizer* m_Data;

        /// <summary>
        /// Returns the result code from the last tokenize call.
        /// </summary>
        public int ResultCode => m_Data->ResultCode;

        public JsonTokenizer(Allocator label = SerializationConfiguration.DefaultAllocatorLabel)
        {
            m_Label = label;
            m_Data = (UnsafeJsonTokenizer*) UnsafeUtility.Malloc(sizeof(UnsafeJsonTokenizer), UnsafeUtility.AlignOf<UnsafeJsonTokenizer>(), label);
            UnsafeUtility.MemClear(m_Data, sizeof(UnsafeJsonTokenizer));
            *m_Data = new UnsafeJsonTokenizer(label);
        }
        
        public void Dispose()
        {
            if (null == m_Data)
                return;

            m_Data->Dispose();
            UnsafeUtility.Free(m_Data, m_Label);
            m_Data = null;
        }

        /// <summary>
        /// Initializes the tokenizer for re-use.
        /// </summary>
        public readonly void Reset()
        {
            m_Data->Reset();
        }

        /// <summary>
        /// Writes <see cref="T:Unity.Serialization.Token" /> objects to the internal buffer.
        /// </summary>
        /// <param name="stream">The output stream to write to.</param>
        /// <param name="buffer">A character array containing the input json data to tokenize.</param>
        /// <param name="start">The index of ptr at which to begin reading.</param>
        /// <param name="count">The maximum number of characters to read.</param>
        /// <param name="isFinalBlock">A value indicating if this is the final block of characters from a stream. This will trigger an error for any unclosed scopes.</param>
        /// <returns>The number of characters that have been read.</returns>
        public readonly int Write(JsonTokenStream stream, UnsafeBuffer<char> buffer, int start, int count, bool isFinalBlock = false)
        {
            if (start + count > buffer.Length)
                throw new ArgumentOutOfRangeException();

            return m_Data->Write(stream.GetUnsafePtr(), (ushort*)buffer.Buffer, start, count, isFinalBlock);
        }
        
        public void CheckAndThrowInvalidJsonExceptions()
        {
            if (m_Data->ResultCode == ResultInvalidInput)
            {
                // No validation pass was performed.
                // The tokenizer has failed with something that was structurally invalid.
                throw new InvalidJsonException($"Input json was structurally invalid. Try with {nameof(JsonValidationType)}=[Standard or Simple]")
                {
                    Line = -1,
                    Character = -1
                };
            }
        }
    }
}