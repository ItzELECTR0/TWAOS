using System;
using System.IO;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace Unity.Serialization.Json
{
    [BurstCompile]
    unsafe struct UnsafeSerializedObjectReader : IDisposable
    {
        delegate void GetNextBlockDelegate(IntPtr self);
        
        static readonly SharedStatic<IntPtr> s_GetNextBlockTrampoline = SharedStatic<IntPtr>.GetOrCreate<GetNextBlockDelegate>();
        
        static object s_GetNextBlockDelegateReference;
        
        internal static void CreateBurstDelegates()
        {
            if (s_GetNextBlockDelegateReference == null)
            {
                var trampoline = new GetNextBlockDelegate(GetNextBlockDelegateInMono);
                s_GetNextBlockDelegateReference = trampoline;
                s_GetNextBlockTrampoline.Data = Marshal.GetFunctionPointerForDelegate(trampoline);
            }
        }
        
        readonly bool m_LeaveOutputOpen;

        JsonTokenStream m_JsonTokenStream;
        JsonTokenizer m_JsonTokenizer;
        JsonValidator m_JsonValidator;
        NodeParser m_NodeParser;
        PackedBinaryStream m_PackedBinaryStream;
        PackedBinaryWriter m_PackedBinaryWriter;

        GCHandle m_StreamBlockReader;
        
        UnsafeBuffer<char> m_InitialBlock;
        UnsafeBuffer<char> m_CurrentBlock;

        bool m_CurrentBlockIsFinal;

        bool m_RequiresExplicitExceptionHandling;
        bool m_IsInvalid;
        JsonValidationResult m_ValidationResult;

        /// <summary>
        /// If this flag is true. No exceptions are thrown unless <see cref="CheckAndThrowInvalidJsonException"/> is called.
        /// </summary>
        public bool RequiresExplicitExceptionHandling
        {
            get => m_RequiresExplicitExceptionHandling;
            set => m_RequiresExplicitExceptionHandling = value;
        }
        
        static PackedBinaryStream OpenBinaryStreamWithConfiguration(SerializedObjectReaderConfiguration configuration, Allocator label)
        {
            if (configuration.TokenBufferSize < 16)
                throw new ArgumentException("TokenBufferSize < 16");

            if (configuration.OutputBufferSize < 16)
                throw new ArgumentException("OutputBufferSize < 16");

            return new PackedBinaryStream(configuration.TokenBufferSize, configuration.OutputBufferSize, label);
        }

        public UnsafeSerializedObjectReader(Stream input, SerializedObjectReaderConfiguration configuration, Allocator label = SerializationConfiguration.DefaultAllocatorLabel, bool leaveInputOpen = true)
            : this(input, OpenBinaryStreamWithConfiguration(configuration, label), configuration, label, leaveInputOpen, false)
        {

        }
        
        public UnsafeSerializedObjectReader(Stream input, PackedBinaryStream output, SerializedObjectReaderConfiguration configuration, Allocator label = SerializationConfiguration.DefaultAllocatorLabel, bool leaveInputOpen = true)
            : this(input, output, configuration, label, leaveInputOpen, true)
        {

        }

        internal UnsafeSerializedObjectReader(Stream input, PackedBinaryStream output, SerializedObjectReaderConfiguration configuration, Allocator label = SerializationConfiguration.DefaultAllocatorLabel, bool leaveInputOpen = true, bool leaveOutputOpen = true)
        {
            if (configuration.BlockBufferSize < 16)
                throw new ArgumentException("BlockBufferSize < 16");

            if (configuration.TokenBufferSize < 16)
                throw new ArgumentException("TokenBufferSize < 16");
            
            m_LeaveOutputOpen = leaveOutputOpen;
            
            m_JsonTokenStream = new JsonTokenStream(configuration.TokenBufferSize, label);
            m_JsonTokenizer = new JsonTokenizer(label);
            m_JsonValidator = default;

            switch (configuration.ValidationType)
            {
                case JsonValidationType.Standard:
                case JsonValidationType.Simple:
                    m_JsonValidator = new JsonValidator(configuration.ValidationType, label);
                    break;
            }
            
            m_NodeParser = new NodeParser(m_JsonTokenStream, configuration.NodeBufferSize, label);
            m_PackedBinaryStream = output;
            m_PackedBinaryWriter = new PackedBinaryWriter(m_JsonTokenStream, m_PackedBinaryStream, label)
            {
                StripStringEscapeCharacters = configuration.StripStringEscapeCharacters
            };

            var reader = configuration.UseReadAsync 
                ? (IUnsafeStreamBlockReader) new AsyncBlockReader(input, configuration.BlockBufferSize, leaveInputOpen) 
                : (IUnsafeStreamBlockReader) new SyncBlockReader(input, configuration.BlockBufferSize, leaveInputOpen);
            
            m_StreamBlockReader = GCHandle.Alloc(reader);
            
            m_InitialBlock = default;
            m_CurrentBlock = default;
            m_CurrentBlockIsFinal = false;
            m_IsInvalid = false;
            m_ValidationResult = default;
            m_RequiresExplicitExceptionHandling = false;
        }
        
        public UnsafeSerializedObjectReader(char* buffer, int length, SerializedObjectReaderConfiguration configuration, Allocator label = SerializationConfiguration.DefaultAllocatorLabel)
            : this(buffer, length, OpenBinaryStreamWithConfiguration(configuration, label), configuration, label)
        {
        }
        
        public UnsafeSerializedObjectReader(char* buffer, int length, PackedBinaryStream output, SerializedObjectReaderConfiguration configuration, Allocator label = SerializationConfiguration.DefaultAllocatorLabel)
            : this(buffer, length, output, configuration, label, true)
        {
        }
        
        internal UnsafeSerializedObjectReader(char* buffer, int length, PackedBinaryStream output, SerializedObjectReaderConfiguration configuration, Allocator label = SerializationConfiguration.DefaultAllocatorLabel, bool leaveOutputOpen = true)
        {
            if (configuration.BlockBufferSize < 16)
                throw new ArgumentException("BlockBufferSize < 16");

            if (configuration.TokenBufferSize < 16)
                throw new ArgumentException("TokenBufferSize < 16");
            
            m_LeaveOutputOpen = leaveOutputOpen;
            
            m_JsonTokenStream = new JsonTokenStream(configuration.TokenBufferSize, label);
            m_JsonTokenizer = new JsonTokenizer(label);
            m_JsonValidator = default;

            switch (configuration.ValidationType)
            {
                case JsonValidationType.Standard:
                case JsonValidationType.Simple:
                    m_JsonValidator = new JsonValidator(configuration.ValidationType, label);
                    break;
            }
            
            m_NodeParser = new NodeParser(m_JsonTokenStream, configuration.NodeBufferSize, label);
            m_PackedBinaryStream = output;
            m_PackedBinaryWriter = new PackedBinaryWriter(m_JsonTokenStream, m_PackedBinaryStream, label)
            {
                StripStringEscapeCharacters = configuration.StripStringEscapeCharacters
            };

            m_StreamBlockReader = default;
            
            m_InitialBlock = new UnsafeBuffer<char>(buffer, length);
            m_CurrentBlock = default;
            m_CurrentBlockIsFinal = false;
            m_IsInvalid = false;
            m_ValidationResult = default;
            m_RequiresExplicitExceptionHandling = false;
        }

        public void Dispose()
        {
            m_JsonTokenStream.Dispose();
            m_JsonTokenizer.Dispose();
            m_JsonValidator.Dispose();
            m_NodeParser.Dispose();
            
            if (!m_LeaveOutputOpen)
                m_PackedBinaryStream.Dispose();
            
            m_PackedBinaryWriter.Dispose();

            DisposeManagedResources();
        }

        [BurstDiscard]
        void DisposeManagedResources()
        {
            if (m_StreamBlockReader.IsAllocated)
            {
                (m_StreamBlockReader.Target as IDisposable)?.Dispose();
                m_StreamBlockReader.Free();
            }
        }

        public void Reset()
        {
            m_JsonTokenStream.Reset();
            m_JsonTokenizer.Reset();
            m_JsonValidator.Reset();
            m_NodeParser.Reset();
            m_PackedBinaryStream.Clear();
            m_PackedBinaryWriter.Seek(0, -1);
        }
        
        public void SetSource(char* ptr, int length)
        {
            Reset();
            
            m_InitialBlock = new UnsafeBuffer<char>(ptr, length);
            m_CurrentBlock = default;
        }
        
        /// <summary>
        /// Discards completed data from the buffers.
        /// </summary>
        public void DiscardCompleted()
        {
            m_PackedBinaryStream.DiscardCompleted();
        }
        
        /// <summary>
        /// Advances the reader to the given node type, ignoring depth/scope.
        /// </summary>
        /// <param name="node">The node type to stop at.</param>
        /// <returns>The node type the parser stopped at.</returns>
        public NodeType Step(NodeType node = NodeType.Any)
        {
            ReadInternal(node, NodeParser.IgnoreParent);
            return m_NodeParser.NodeType;
        }

        /// <summary>
        /// Advances the reader to the given node type, ignoring depth/scope.
        /// </summary>
        /// <param name="view">The view at the returned node type.</param>
        /// <param name="node">The node type to stop at.</param>
        /// <returns>The node type the parser stopped at.</returns>
        public NodeType Step(out SerializedValueView view, NodeType node = NodeType.Any)
        {
            view = ReadInternal(node, NodeParser.IgnoreParent);
            return m_NodeParser.NodeType;
        }
        
        /// <summary>
        /// Reads the next node in the stream.
        /// </summary>
        /// <returns>The <see cref="SerializedObjectView"/> that was read.</returns>
        /// <exception cref="InvalidOperationException">The reader state is invalid.</exception>
        public NodeType Read()
        {
            return Read(out _);
        }
        
        /// <summary>
        /// Reads the next node in the stream, respecting depth/scope.
        /// </summary>
        /// <param name="node">The node type to stop at.</param>
        /// <returns>The node type the parser stopped at.</returns>
        public NodeType Read(NodeType node = NodeType.Any)
        {
            ReadInternal(node, m_NodeParser.TokenParentIndex);
            return m_NodeParser.NodeType;
        }
        
        /// <summary>
        /// Reads the next node in the stream, respecting depth/scope.
        /// </summary>
        /// <param name="view">The view at the returned node type.</param>
        /// <param name="node">The node type to stop at.</param>
        /// <returns>The node type the parser stopped at.</returns>
        public NodeType Read(out SerializedValueView view, NodeType node = NodeType.Any)
        {
            view = ReadInternal(node, m_NodeParser.TokenParentIndex);
            return m_NodeParser.NodeType;
        }
        
        /// <summary>
        /// Reads the next node as a <see cref="SerializedObjectView"/>
        /// </summary>
        /// <returns>The <see cref="SerializedObjectView"/> that was read.</returns>
        /// <exception cref="InvalidOperationException">The reader state is invalid.</exception>
        public SerializedObjectView ReadObject()
        {
            FillBuffers();

            if (!CheckNextTokenType(TokenType.Object))
                throw new InvalidOperationException($"Invalid token read Expected=[{TokenType.Object}] but Received=[{GetNextTokenType()}]");
            
            Read(out var view);
            return view.AsObjectView();
        }
        
        public SerializedMemberView ReadMember()
        {
            FillBuffers();

            var nextTokenType = GetNextTokenType();
            if (nextTokenType != TokenType.String && nextTokenType != TokenType.Primitive)
            {
                throw new InvalidOperationException($"Invalid token read Expected=[{TokenType.String}|{TokenType.Primitive}] but Received=[{GetNextTokenType()}]");
            }
            Read(out var view);
            return view.AsMemberView();
        }
        
        /// <summary>
        /// Reads the next node as a member, respecting depth/scope and adds it to the given <see cref="SerializedMemberViewCollection"/>.
        /// </summary>
        /// <param name="collection">The collection to add the member to.</param>
        public void ReadMember(SerializedMemberViewCollection collection)
        {
            collection.Add(ReadMember());
        }
        
        /// <summary>
        /// Reads the next node as an array element.
        /// </summary>
        /// <param name="view">The view of the array element.</param>
        /// <returns>True if the element was successfully read, false otherwise.</returns>
        public bool ReadArrayElement(out SerializedValueView view)
        {
            if (!CheckArrayElement())
            {
                view = default;
                return false;
            }

            view = ReadInternal(NodeType.Any, m_NodeParser.TokenParentIndex);

            if (m_NodeParser.NodeType != NodeType.Any)
                return true;

            view = default;
            return false;
        }
        
        /// <summary>
        /// Reads the next <see cref="count"/> elements of an array and writes views to the given buffer.
        /// </summary>
        /// <param name="views">The buffer to write the views to.</param>
        /// <param name="count">The number of elements to read.</param>
        /// <returns>The number of elements read.</returns>
        public int ReadArrayElementBatch(SerializedValueView* views, int count)
        {
            if (!CheckArrayElement())
                return 0;

            count = ReadInternalBatch(views, count, NodeType.Any, m_NodeParser.TokenParentIndex);
            return count;
        }

        SerializedValueView ReadInternal(NodeType type, int parent)
        {
            for (;;)
            {
                // Parse and tokenize the input stream.
                if (FillBuffers())
                {
                    // Remap the parent if the internal buffers have shifted.
                    if (parent >= 0) parent = m_JsonTokenStream.Discard[parent];
                }

                var parserStart = m_NodeParser.TokenNextIndex - 1;
                var writerStart = m_PackedBinaryStream.TokenNextIndex - 1;

                m_NodeParser.Step(type, parent);
                
                m_PackedBinaryWriter.Write(m_CurrentBlock, m_NodeParser.TokenNextIndex - m_PackedBinaryWriter.TokenNextIndex);

                if (m_NodeParser.NodeType == NodeType.None && !IsEmpty(m_CurrentBlock))
                    continue;

                var node = m_NodeParser.Node;
                return node == -1 ? default : m_PackedBinaryStream.GetView(GetViewIndex(node, parserStart, writerStart));
            }
        }
        
        int ReadInternalBatch(SerializedValueView* views, int count, NodeType type, int parent)
        {
            var index = 0;

            for (;;)
            {
                // Parse and tokenize the input stream.
                if (FillBuffers())
                {
                    // Remap the parent if the internal buffers have shifted.
                    if (parent >= 0) parent = m_JsonTokenStream.Discard[parent];
                }

                var parserStart = m_NodeParser.TokenNextIndex - 1;
                var writerStart = m_PackedBinaryStream.TokenNextIndex - 1;

                var batchCount = m_NodeParser.StepBatch(count - index, type, parent);

                m_PackedBinaryWriter.Write(m_CurrentBlock, m_NodeParser.TokenNextIndex - m_PackedBinaryWriter.TokenNextIndex);

                for (var i = 0; i < batchCount; i++)
                    views[index + i] = m_PackedBinaryStream.GetView(GetViewIndex(m_NodeParser.Nodes[i], parserStart, writerStart));

                index += batchCount;

                if (m_NodeParser.NodeType == NodeType.None && !IsEmpty(m_CurrentBlock))
                    continue;

                return index;
            }
        }
        
        static bool IsEmpty(UnsafeBuffer<char> buffer)
        {
            return buffer.Buffer == null || buffer.Length == 0;
        }
        
        bool FillBuffers()
        {
            if (m_NodeParser.TokenNextIndex < m_JsonTokenStream.TokenNextIndex || m_NodeParser.NodeType != NodeType.None)
                return false;

            if (m_StreamBlockReader.IsAllocated)
            {
                var executedByMono = false;
                
                TryGetNextBlock(ref executedByMono);

                if (!executedByMono)
                {
                    // We are running from burst. Call an extern function to read from the `System.IO.Stream`
                    fixed (void* self = &this)
                        new FunctionPointer<GetNextBlockDelegate>(s_GetNextBlockTrampoline.Data).Invoke((IntPtr)self);
                }
            }
            else
            {
                m_CurrentBlock = m_InitialBlock;
                m_InitialBlock = default;
                m_CurrentBlockIsFinal = true;
            }
                
            if (IsEmpty(m_CurrentBlock))
            {
                m_CurrentBlock = default;
                
                // We need to perform off one final write call to trigger validation.
                m_JsonTokenizer.Write(m_JsonTokenStream, m_CurrentBlock, 0, 0, true);
                Validate();
                return false;
            }
            
            m_JsonTokenStream.DiscardCompleted();
            m_NodeParser.Seek(m_JsonTokenStream.TokenNextIndex, m_JsonTokenStream.TokenParentIndex);
            m_PackedBinaryWriter.Seek(m_JsonTokenStream.TokenNextIndex, m_PackedBinaryWriter.TokenParentIndex != -1
                ? m_JsonTokenStream.Discard[m_PackedBinaryWriter.TokenParentIndex]
                : -1);
            
            m_JsonTokenizer.Write(m_JsonTokenStream, m_CurrentBlock, 0, m_CurrentBlock.Length, m_CurrentBlockIsFinal);
            Validate();
            return true;
        }

        [BurstDiscard]
        void TryGetNextBlock(ref bool result)
        {
            result = true;
            var reader = m_StreamBlockReader.Target as IUnsafeStreamBlockReader;
            var block = reader.GetNextBlock();

            m_CurrentBlock = block.Block;
            m_CurrentBlockIsFinal = block.IsFinal;
        }
        
        [BurstDiscard]
        [MonoPInvokeCallback(typeof(GetNextBlockDelegate))]
        static void GetNextBlockDelegateInMono(IntPtr target)
        {
            try
            {
                var self = (UnsafeSerializedObjectReader*) target.ToPointer();
                var reader = self->m_StreamBlockReader.Target as IUnsafeStreamBlockReader;
                var block = reader.GetNextBlock();

                self->m_CurrentBlock = block.Block;
                self->m_CurrentBlockIsFinal = block.IsFinal;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        void Validate()
        {
            if (!m_JsonValidator.IsCreated)
            {
                // When running without proper validation we rely on the tokenizer to check if the input is valid.
                if (m_JsonTokenizer.ResultCode == JsonTokenizer.ResultInvalidInput)
                {
                    m_IsInvalid = true;
                    
                    if (!m_RequiresExplicitExceptionHandling)
                        CheckAndThrowInvalidJsonException();
                }
                
                return;
            }

            m_ValidationResult = m_JsonValidator.Validate(m_CurrentBlock, 0, m_CurrentBlock.Length);

            if (!m_ValidationResult.IsValid())
            {
                // The JSON is considered invalid at this point. However we have a few special cases to consider.
                if (m_ValidationResult.ActualType == JsonType.EOF && !m_CurrentBlockIsFinal)
                {
                    // The last received token was an end of stream token but we are still waiting on more characters.
                    // We can safely ignore this error for now.
                }
                else if (m_JsonTokenStream.TokenNextIndex == 1 && m_JsonTokenStream.Tokens[0].Type == TokenType.Primitive)
                {
                    // This is a single primitive value. We can deserialize safely and accept this as valid.
                }
                else
                {
                    m_IsInvalid = true;
                    
                    if (!m_RequiresExplicitExceptionHandling)
                        CheckAndThrowInvalidJsonException();
                }
            }
        }

        [BurstDiscard]
        public void CheckAndThrowInvalidJsonException()
        {
            if (m_IsInvalid)
            {
                if (m_JsonValidator.IsCreated)
                {
                    throw new InvalidJsonException(m_ValidationResult)
                    {
                        Line = m_ValidationResult.LineCount,
                        Character = m_ValidationResult.CharCount
                    };
                }

                throw new InvalidJsonException($"Input json was structurally invalid. Try with {nameof(JsonValidationType)}=[Standard or Simple]")
                {
                    Line = -1,
                    Character = -1
                };
            }
        }

        int GetViewIndex(int node, int inputTokenStart, int binaryTokenStart)
        {
            if (node == -1)
                return -1;

            var data = m_PackedBinaryStream.GetUnsafePtr();

            if (node >= inputTokenStart)
            {
                // This is a newly written token.
                // Since we know tokens are written in order; we can simply compute the offset.
                var offset = m_NodeParser.TokenNextIndex - node;
                return data->TokenNextIndex - offset;
            }

            // This is a previously written token.
            // Since we know we can never discard an incomplete token.
            // We must walk up the tree the same number of times for both streams to find the correct token.
            var binaryIndex = binaryTokenStart;

            var binaryTokens = m_PackedBinaryStream.GetUnsafePtr()->Tokens;
            while (inputTokenStart != node)
            {
                if (inputTokenStart == -1)
                    throw new IndexOutOfRangeException();
                
                inputTokenStart = m_JsonTokenStream.Tokens[inputTokenStart].Parent;
                binaryIndex = binaryTokens[binaryIndex].Parent;
            }

            return binaryIndex;
        }
        
        TokenType GetNextTokenType()
        {
            return m_NodeParser.TokenNextIndex >= m_JsonTokenStream.TokenNextIndex ? TokenType.Undefined : m_JsonTokenStream.Tokens[m_NodeParser.TokenNextIndex].Type;
        }
        
        bool CheckNextTokenType(TokenType type)
        {
            return m_NodeParser.TokenNextIndex < m_JsonTokenStream.TokenNextIndex && m_JsonTokenStream.Tokens[m_NodeParser.TokenNextIndex].Type == type;
        }
        
        bool CheckArrayElement()
        {
            if (m_NodeParser.Node == -1)
                return false;

            return CheckNextTokenParent(m_NodeParser.NodeType == NodeType.BeginArray ? m_NodeParser.Node : m_JsonTokenStream.Tokens[m_NodeParser.Node].Parent);
        }
        
        bool CheckNextTokenParent(int parent)
        {
            if (m_NodeParser.TokenNextIndex >= m_JsonTokenStream.TokenNextIndex)
            {
                return false;
            }

            return m_JsonTokenStream.Tokens[m_NodeParser.TokenNextIndex].Parent == parent;
        }
    }
}