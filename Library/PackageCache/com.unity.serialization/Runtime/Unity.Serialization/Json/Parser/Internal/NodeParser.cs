using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Serialization.Json
{
    unsafe struct UnsafeNodeParser : IDisposable
    {
        public const int IgnoreParent = -2;
        
        readonly Allocator m_Label;
        readonly UnsafeJsonTokenStream* m_Stream;
        
        public int TokenNextIndex;
        public int TokenParentIndex;

        public int* Nodes;
        public int NodeLength;
        public int NodeNextIndex;

        public NodeType NodeType;
        
        int m_TargetNodeCount;
        NodeType m_TargetNodeType;
        int m_TargetParentIndex;

        public UnsafeNodeParser(UnsafeJsonTokenStream* stream, int batchSize, Allocator label)
        {
            m_Label = label;

            m_Stream = stream;
            TokenNextIndex = 0;
            TokenParentIndex = -1;
            
            Nodes = (int*) UnsafeUtility.Malloc(sizeof(int) * batchSize, 4, m_Label);
            NodeLength = batchSize;
            NodeNextIndex = 0;

            NodeType = NodeType.None;
            
            m_TargetNodeCount = 0;
            m_TargetNodeType = NodeType.None;
            m_TargetParentIndex = -1;
        }

        public void Dispose()
        {
            UnsafeUtility.Free(Nodes, m_Label);
            Nodes = null;
        }

        public void Reset()
        {
            NodeNextIndex = 0;
            NodeType = NodeType.None;
            TokenNextIndex = 0;
            TokenParentIndex = -1;
        }
        
        public void Seek(int index, int parent)
        {
            TokenNextIndex = index;
            TokenParentIndex = parent;
        }

        /// <summary>
        /// Reads the next node from the input stream and advances the position by one.
        /// </summary>
        public NodeType Step()
        {
            Step(NodeType.Any);
            return NodeType;
        }

        /// <summary>
        /// Reads until the given node type and advances the position.
        /// <param name="type">The node type to break at.</param>
        /// <param name="parent">The minimum parent to break at.</param>
        /// </summary>
        public void Step(NodeType type, int parent = IgnoreParent)
        {
            StepBatch(1, type, parent);
        }

        /// <summary>
        /// Reads until the given number of matching nodes have been read.
        /// </summary>
        /// <param name="count">The maximum number of elements of the given type/parent to read.</param>
        /// <param name="type">The node type to break at.</param>
        /// <param name="parent">The minimum parent to break at.</param>
        /// <returns>The number of batch elements that have been read.</returns>
        public int StepBatch(int count, NodeType type, int parent = IgnoreParent)
        {
            NodeNextIndex = 0;
            
            m_TargetNodeCount = count;
            m_TargetNodeType = type;
            m_TargetParentIndex = parent;
            
            if (NodeLength < m_TargetNodeCount)
            {
                UnsafeUtility.Free(Nodes, m_Label);
                
                NodeLength = m_TargetNodeCount;
                Nodes = (int*) UnsafeUtility.Malloc(sizeof(int) * NodeLength, 4, m_Label);
            }

            for (; TokenNextIndex < m_Stream->TokenNextIndex; TokenNextIndex++)
            {
                var node = NodeType.None;

                var token = m_Stream->Tokens[TokenNextIndex];

                while (m_Stream->Tokens[TokenNextIndex].Parent < TokenParentIndex)
                {
                    var index = TokenParentIndex;

                    node = PopToken();

                    if (Evaluate(node, index))
                    {
                        if (TokenParentIndex < m_TargetParentIndex)
                        {
                            TokenParentIndex = index;
                        }

                        NodeType = node == NodeType.None ? NodeType.Any : node;
                        return NodeNextIndex;
                    }
                }

                var nodeIndex = TokenNextIndex;

                switch (token.Type)
                {
                    case TokenType.Array:
                    case TokenType.Object:
                    {
                        node |= token.Type == TokenType.Array ? NodeType.BeginArray : NodeType.BeginObject;
                        TokenParentIndex = TokenNextIndex;
                    }
                    break;

                    case TokenType.Primitive:
                    case TokenType.String:
                    {
                        if (token.End != -1)
                        {
                            node |= token.Type == TokenType.Primitive ? NodeType.Primitive : NodeType.String;

                            while (token.Start == -1)
                            {
                                nodeIndex = token.Parent;
                                token = m_Stream->Tokens[nodeIndex];
                            }

                            if (token.Parent == -1 || m_Stream->Tokens[token.Parent].Type == TokenType.Object)
                            {
                                node |= NodeType.ObjectKey;
                                TokenParentIndex = TokenNextIndex;
                            }
                        }
                    }
                    break;

                    case TokenType.Comment:
                    {
                        if (token.End != -1)
                        {
                            node |= NodeType.Comment;

                            while (token.Start == -1)
                            {
                                nodeIndex = token.Parent;
                                token = m_Stream->Tokens[nodeIndex];
                            }
                        }
                    }
                    break;
                }

                if (Evaluate(node, nodeIndex))
                {
                    TokenNextIndex++;
                    NodeType = node == NodeType.None ? NodeType.Any : node;
                    return NodeNextIndex;
                }
            }

            while (TokenParentIndex >= 0)
            {
                var index = TokenParentIndex;
                var token = m_Stream->Tokens[index];

                if (token.End == -1 && (token.Type == TokenType.Object || token.Type == TokenType.Array))
                {
                    NodeType = NodeType.None;
                    return NodeNextIndex;
                }

                var node = PopToken();

                if (Evaluate(node, index))
                {
                    if (TokenParentIndex < parent)
                    {
                        TokenParentIndex = index;
                    }

                    NodeType = node == NodeType.None ? NodeType.Any : node;
                    return NodeNextIndex;
                }
            }

            NodeType = NodeType.None;
            return NodeNextIndex;
        }

        /// <summary>
        /// Evaluate user instruction to determine if we should break the parsing.
        ///
        /// @TODO Cleanup; far too many checks happening here
        /// </summary>
        bool Evaluate(NodeType node, int index)
        {
            if (TokenParentIndex <= m_TargetParentIndex)
            {
                if (node == NodeType.None || (node & m_TargetNodeType) == node && TokenParentIndex == m_TargetParentIndex)
                {
                    Nodes[NodeNextIndex++] = index;

                    if (NodeNextIndex < m_TargetNodeCount)
                    {
                        return false;
                    }
                }

                return true;
            }

            if (node == NodeType.None)
            {
                return false;
            }

            if ((node & m_TargetNodeType) != NodeType.None && (m_TargetParentIndex == IgnoreParent || m_TargetParentIndex >= TokenParentIndex))
            {
                Nodes[NodeNextIndex++] = index;

                if (NodeNextIndex >= m_TargetNodeCount)
                {
                    return true;
                }
            }

            return false;
        }

        NodeType PopToken()
        {
            var node = NodeType.None;
            var token = m_Stream->Tokens[TokenParentIndex];

            switch (token.Type)
            {
                case TokenType.Array:
                    node = NodeType.EndArray;
                    break;
                case TokenType.Object:
                    node = NodeType.EndObject;
                    break;
            }

            var parentIndex = token.Parent;

            while (parentIndex >= 0)
            {
                var parent = m_Stream->Tokens[parentIndex];

                if (parent.Type != TokenType.Primitive && parent.Type != TokenType.String && parent.Type != TokenType.Comment)
                {
                    break;
                }

                if (parent.Start != -1 || parent.Parent == -1)
                {
                    break;
                }

                parentIndex = parent.Parent;
            }

            TokenParentIndex = parentIndex;
            return node;
        }
    }
    
    unsafe struct NodeParser : IDisposable
    {
        public const int IgnoreParent = UnsafeNodeParser.IgnoreParent;
        
        const int k_DefaultBatchSize = 64;

        readonly Allocator m_Label;
        
        [NativeDisableUnsafePtrRestriction] UnsafeNodeParser* m_Data;

        public int* Nodes => m_Data->Nodes;
        public int NodeNextIndex => m_Data->NodeNextIndex;
        public NodeType NodeType => m_Data->NodeType;
        public int Node => m_Data->NodeNextIndex <= 0 ? -1 : m_Data->Nodes[m_Data->NodeNextIndex - 1];
        public int TokenNextIndex => m_Data->TokenNextIndex;
        public int TokenParentIndex => m_Data->TokenParentIndex;

        public NodeParser(JsonTokenStream stream, Allocator label) : this(stream, k_DefaultBatchSize, label)
        {
        }

        public NodeParser(JsonTokenStream stream, int batchSize, Allocator label)
        {
            if (batchSize < 1)
                throw new ArgumentException("batchSize < 1");
            
            m_Label = label;
            m_Data = (UnsafeNodeParser*) UnsafeUtility.Malloc(sizeof(UnsafeNodeParser), UnsafeUtility.AlignOf<UnsafeNodeParser>(), label);
            *m_Data = new UnsafeNodeParser(stream.GetUnsafePtr(), batchSize, label);
        }

        public void Reset()
        {
            m_Data->Reset();
        }

        /// <summary>
        /// Seeks the parser to the given token/parent combination.
        /// </summary>
        public void Seek(int index, int parent)
        {
            m_Data->Seek(index, parent);
        }

        /// <summary>
        /// Reads the next node from the input stream and advances the position by one.
        /// </summary>
        public NodeType Step()
        {
            return m_Data->Step();
        }

        /// <summary>
        /// Reads until the given node type and advances the position.
        /// <param name="type">The node type to break at.</param>
        /// <param name="parent">The minimum parent to break at.</param>
        /// </summary>
        public void Step(NodeType type, int parent = UnsafeNodeParser.IgnoreParent)
        {
            m_Data->Step(type, parent);
        }

        /// <summary>
        /// Reads until the given number of matching nodes have been read.
        /// </summary>
        /// <param name="count">The maximum number of elements of the given type/parent to read.</param>
        /// <param name="type">The node type to break at.</param>
        /// <param name="parent">The minimum parent to break at.</param>
        /// <returns>The number of batch elements that have been read.</returns>
        public int StepBatch(int count, NodeType type, int parent = UnsafeNodeParser.IgnoreParent)
        {
            return m_Data->StepBatch(count, type, parent);
        }

        public void Dispose()
        {
            m_Data->Dispose();
            UnsafeUtility.Free(m_Data, m_Label);
            m_Data = null;
        }
    }
}