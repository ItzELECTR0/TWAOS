using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Serialization.Json
{
    [StructLayout(LayoutKind.Sequential)]
    struct Token
    {
        /// <summary>
        /// The token type.
        /// </summary>
        public TokenType Type;

        /// <summary>
        /// The parent token. This can be an object, array, member, or part of a split token.
        /// </summary>
        public int Parent;

        /// <summary>
        /// The start position in the original input data.
        /// </summary>
        public int Start;

        /// <summary>
        /// The end position in the original input data.
        ///
        /// This points to the character after the data.
        /// </summary>
        public int End;

        public override string ToString()
        {
            return $"Type=[{Type}] Range=[{Start}..{End}] Parent=[{Parent}]";
        }
    }
    
    unsafe struct UnsafeJsonTokenStream : IDisposable
    {
        /// <summary>
        /// Special start value to signify this is a partial token continuation.
        /// </summary>
        public const int PartialTokenStart = -1;
        
        /// <summary>
        /// Special end value to signify there is another part to follow.
        /// </summary>
        public const int PartialTokenEnd = -1;
        
        /// <summary>
        /// The default depth limit for discarding completed tokens.
        /// </summary>
        public const int DefaultDiscardDepthLimit = 128;
        
        readonly Allocator m_Label;
        
        public Token* Tokens;
        public int TokenCapacity;
        public int TokenNextIndex;
        public int TokenParentIndex;
        public int* Discard;

        public UnsafeJsonTokenStream(int initialTokenCapacity, Allocator label)
        {
            m_Label = label;
            Tokens = (Token*) UnsafeUtility.Malloc(initialTokenCapacity * sizeof(Token), 4, label);
            Discard = (int*) UnsafeUtility.Malloc(initialTokenCapacity * sizeof(int), 4, label);
            TokenCapacity = initialTokenCapacity;
            TokenNextIndex = 0;
            TokenParentIndex = -1;
        }
        
        public void Dispose()
        {
            UnsafeUtility.Free(Tokens, m_Label);
            UnsafeUtility.Free(Discard, m_Label);
            Tokens = null;
            Discard = null;
        }

        public void Add(Token token)
        {
            if (TokenNextIndex >= TokenCapacity)
                Grow();

            Tokens[TokenNextIndex++] = token;
        }

        public void Reset()
        {
            TokenNextIndex = 0;
            TokenParentIndex = -1;
        }
        
        public int DiscardCompleted(int depth = DefaultDiscardDepthLimit)
        {
            if (TokenNextIndex == 0)
                return 0;

            var stack = stackalloc int[depth];
            var sp = -1;

            var index = TokenNextIndex - 1;

            for (;;)
            {
                if (index == -1)
                {
                    break;
                }

                var token = Tokens[index];

                if (token.Start != PartialTokenStart)
                {
                    // Support partial tokens
                    if (token.End == PartialTokenEnd && (token.Type == TokenType.Primitive || token.Type == TokenType.String))
                    {
                        var partToken = token;
                        var partIndex = index;
                        var partCount = 1;

                        while (partToken.End == -1 && partIndex < TokenNextIndex - 1)
                        {
                            partCount++;
                            partToken = Tokens[++partIndex];
                        }

                        if (sp + partCount >= depth)
                        {
                            // throw new StackOverflowException($"Tokenization depth limit of {depth} exceeded.");
                            return -1;
                        }

                        for (var i = partCount - 1; i >= 0; i--)
                        {
                            stack[++sp] = index + i;
                        }
                    }
                    else
                    {
                        if (sp + 1 >= depth)
                        {
                            // throw new StackOverflowException($"Tokenization depth limit of {depth} exceeded.");
                            return -1;
                        }

                        stack[++sp] = index;
                    }
                }

                index = token.Parent;
            }

            TokenNextIndex = sp + 1;

            for (var i = 0; sp >= 0; i++, sp--)
            {
                index = stack[sp];

                if (TokenParentIndex == index)
                {
                    TokenParentIndex = i;
                }

                var token = Tokens[index];

                var parentIndex = i - 1;

                if (token.Start != PartialTokenStart)
                {
                    while (parentIndex != -1 && Tokens[parentIndex].Start == PartialTokenStart)
                    {
                        parentIndex--;
                    }
                }

                token.Parent = parentIndex;
                Tokens[i] = token;
                Discard[index] = i;
            }
            
            return 0;
        }

        void Grow()
        {
            var newLength = TokenCapacity * 2;
            Tokens = NativeArrayUtility.Resize(Tokens, TokenCapacity, newLength, 4, m_Label);
            Discard = NativeArrayUtility.Resize(Discard, TokenCapacity, newLength, 4, m_Label);
            TokenCapacity = newLength;
        }
    }

    unsafe struct JsonTokenStream : IDisposable
    {
        const int k_DefaultBufferSize = 1024;
        
        readonly Allocator m_Allocator;
        [NativeDisableUnsafePtrRestriction] UnsafeJsonTokenStream* m_Data;

        public Token* Tokens => m_Data->Tokens;
        public int TokenNextIndex => m_Data->TokenNextIndex;
        public int TokenParentIndex => m_Data->TokenParentIndex;
        public int* Discard => m_Data->Discard;

        internal UnsafeJsonTokenStream* GetUnsafePtr() => m_Data;

        public JsonTokenStream(Allocator allocator) : this(k_DefaultBufferSize, allocator)
        {
            
        }
        
        public JsonTokenStream(int initialLength, Allocator allocator)
        {
            m_Allocator = allocator;
            m_Data = (UnsafeJsonTokenStream*)UnsafeUtility.Malloc(sizeof(UnsafeJsonTokenStream), UnsafeUtility.AlignOf<UnsafeJsonTokenStream>(), m_Allocator);
            *m_Data = new UnsafeJsonTokenStream(initialLength, allocator);
        }

        public void Dispose()
        {
            m_Data->Dispose();
            UnsafeUtility.Free(m_Data, m_Allocator);
            m_Data = null;
        }

        public readonly void Reset()
        {
            m_Data->Reset();
        }

        public readonly void DiscardCompleted(int depth = UnsafeJsonTokenStream.DefaultDiscardDepthLimit)
        {
            m_Data->DiscardCompleted(depth);
        }
    }
}