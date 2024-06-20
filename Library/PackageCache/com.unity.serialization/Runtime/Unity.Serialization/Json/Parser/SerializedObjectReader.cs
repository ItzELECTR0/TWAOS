using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System.IO;
using UnityEngine.Serialization;

namespace Unity.Serialization.Json
{    
    struct BlockInfo
    {
        public UnsafeBuffer<char> Block;
        public bool IsFinal;
    }
    
    interface IUnsafeStreamBlockReader : IDisposable
    {
        /// <summary>
        /// Resets the reader for re-use.
        /// </summary>
        void Reset();
        
        /// <summary>
        /// Returns the next block in the stream.
        /// </summary>
        BlockInfo GetNextBlock();
    }
    
    /// <summary>
    /// Parameters used to configure the <see cref="SerializedObjectReader"/>.
    /// </summary>
    public struct SerializedObjectReaderConfiguration
    {
        /// <summary>
        /// If true, the input stream is read asynchronously. The default is true.
        /// </summary>
        public bool UseReadAsync;

        /// <summary>
        /// The buffer size, in bytes, of the blocks/chunks read from the input stream. The default size is 4096.
        /// </summary>
        public int BlockBufferSize;

        /// <summary>
        /// The internal token buffer size, in tokens. This should be big enough to contain all tokens generated from a block. The default size is 1024.
        /// </summary>
        public int TokenBufferSize;

        /// <summary>
        /// The packed binary output buffer size, in bytes. This should be big enough to contain all string and primitive data for the needed scope. The default size is 4096.
        /// </summary>
        public int OutputBufferSize;

        /// <summary>
        /// The size of the Node buffer for internal reads. For optimal performance, this should be equal to the maximum batch size. The default size is 128.
        /// </summary>
        public int NodeBufferSize;

        /// <summary>
        /// JSON validation type to use. The default is <see cref="JsonValidationType.Standard"/>.
        /// </summary>
        public JsonValidationType ValidationType;

        /// <summary>
        /// Indicates if escape characters should be stripped when reading strings.
        /// </summary>
        public bool StripStringEscapeCharacters;

        /// <summary>
        /// The default parameters used by the <see cref="SerializedObjectReader"/>.
        /// </summary>
        public static readonly SerializedObjectReaderConfiguration Default = new SerializedObjectReaderConfiguration
        {
            UseReadAsync = true,
            BlockBufferSize = 4096,
            TokenBufferSize = 1024,
            OutputBufferSize = 4096,
            NodeBufferSize = 128,
            ValidationType = JsonValidationType.Standard,
            StripStringEscapeCharacters = true
        };
    }
    
    /// <summary>
    /// The <see cref="SerializedObjectReader"/> is the high level API used to deserialize a stream of data.
    /// </summary>
    public unsafe struct SerializedObjectReader : IDisposable
    {
        readonly Allocator m_Label;
        [NativeDisableUnsafePtrRestriction] UnsafeSerializedObjectReader* m_Data;
        
        /// <summary>
        /// If this flag is true. No exceptions are thrown unless <see cref="CheckAndThrowInvalidJsonException"/> is called.
        /// </summary>
        public bool RequiresExplicitExceptionHandling
        {
            get => m_Data->RequiresExplicitExceptionHandling;
            set => m_Data->RequiresExplicitExceptionHandling = value;
        }
        
        static PackedBinaryStream OpenBinaryStreamWithConfiguration(SerializedObjectReaderConfiguration configuration, Allocator label)
        {
            if (configuration.TokenBufferSize < 16)
                throw new ArgumentException("TokenBufferSize < 16");

            if (configuration.OutputBufferSize < 16)
                throw new ArgumentException("OutputBufferSize < 16");

            return new PackedBinaryStream(configuration.TokenBufferSize, configuration.OutputBufferSize, label);
        }

        static Stream OpenFileStreamWithConfiguration(string path, SerializedObjectReaderConfiguration configuration)
        {
            if (configuration.BlockBufferSize < 16)
                throw new ArgumentException("SerializedObjectReaderConfiguration.BlockBufferSize < 16");

            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, configuration.BlockBufferSize, configuration.UseReadAsync ? FileOptions.Asynchronous : FileOptions.None);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedObjectReader"/> class based on the specified configuration.
        /// </summary>
        /// <param name="configuration">The configuration parameters to use for the reader.</param>
        /// <param name="label">The memory allocator label to use.</param>
        public SerializedObjectReader(SerializedObjectReaderConfiguration configuration, Allocator label = SerializationConfiguration.DefaultAllocatorLabel)
            : this(null, 0, OpenBinaryStreamWithConfiguration(configuration, label), configuration, label, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedObjectReader"/> class based on the specified configuration.
        /// </summary>
        /// <param name="output">The output stream.</param>
        /// <param name="configuration">The configuration parameters to use for the reader.</param>
        /// <param name="label">The memory allocator label to use.</param>
        public SerializedObjectReader(PackedBinaryStream output, SerializedObjectReaderConfiguration configuration, Allocator label = SerializationConfiguration.DefaultAllocatorLabel)
            : this(null, 0, output, configuration, label, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedObjectReader"/> class with the specified path.
        /// </summary>
        /// <param name="path">A relative or absolute file path.</param>
        /// <param name="label">The memory allocator label to use.</param>
        public SerializedObjectReader(string path, Allocator label = SerializationConfiguration.DefaultAllocatorLabel)
            : this(path, SerializedObjectReaderConfiguration.Default, label)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedObjectReader"/> class with the specified path and configuration.
        /// </summary>
        /// <param name="path">A relative or absolute file path.</param>
        /// <param name="configuration">The configuration parameters to use for the reader.</param>
        /// <param name="label">The memory allocator label to use.</param>
        public SerializedObjectReader(string path, SerializedObjectReaderConfiguration configuration, Allocator label = SerializationConfiguration.DefaultAllocatorLabel)
            : this(path, OpenBinaryStreamWithConfiguration(configuration, label), configuration, label, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedObjectReader"/> class with the specified path and output stream.
        /// </summary>
        /// <param name="path">A relative or absolute file path.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="label">The memory allocator label to use.</param>
        /// <param name="leaveOutputOpen">True to leave the stream open after the reader object is disposed; otherwise, false.</param>
        public SerializedObjectReader(string path, PackedBinaryStream output, Allocator label = SerializationConfiguration.DefaultAllocatorLabel, bool leaveOutputOpen = true)
            : this(path, output, SerializedObjectReaderConfiguration.Default, label, leaveOutputOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedObjectReader"/> class with the specified path, output stream and configuration.
        /// </summary>
        /// <param name="path">A relative or absolute file path.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="configuration">The configuration parameters to use for the reader.</param>
        /// <param name="label">The memory allocator label to use.</param>
        /// <param name="leaveOutputOpen">True to leave the output stream open after the reader object is disposed; otherwise, false.</param>
        public SerializedObjectReader(string path, PackedBinaryStream output, SerializedObjectReaderConfiguration configuration, Allocator label = SerializationConfiguration.DefaultAllocatorLabel, bool leaveOutputOpen = true)
            : this(OpenFileStreamWithConfiguration(path, configuration), output, configuration, label, false, leaveOutputOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedObjectReader"/> class based on the specified input stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="label">The memory allocator label to use.</param>
        /// <param name="leaveInputOpen">True to leave the input stream open after the reader object is disposed; otherwise, false.</param>
        public SerializedObjectReader(Stream input, Allocator label = SerializationConfiguration.DefaultAllocatorLabel, bool leaveInputOpen = true)
            : this(input, SerializedObjectReaderConfiguration.Default, label, leaveInputOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedObjectReader"/> class based on the specified input stream and output stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="label">The memory allocator label to use.</param>
        /// <param name="leaveInputOpen">True to leave the input stream open after the reader object is disposed; otherwise, false.</param>
        /// <param name="leaveOutputOpen">True to leave the output stream open after the reader object is disposed; otherwise, false.</param>
        public SerializedObjectReader(Stream input, PackedBinaryStream output, Allocator label =  SerializationConfiguration.DefaultAllocatorLabel, bool leaveInputOpen = true, bool leaveOutputOpen = true)
            : this(input, output, SerializedObjectReaderConfiguration.Default, label, leaveInputOpen, leaveOutputOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedObjectReader"/> class based on the specified input stream and configuration.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="configuration">The configuration parameters to use for the reader.</param>
        /// <param name="label">The memory allocator label to use.</param>
        /// <param name="leaveInputOpen">True to leave the input stream open after the reader object is disposed; otherwise, false.</param>
        public SerializedObjectReader(Stream input, SerializedObjectReaderConfiguration configuration, Allocator label = SerializationConfiguration.DefaultAllocatorLabel, bool leaveInputOpen = true)
            : this(input, OpenBinaryStreamWithConfiguration(configuration, label), configuration, label, leaveInputOpen, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedObjectReader"/> class based on the specified input stream, output stream and configuration.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="configuration">The configuration parameters to use for the reader.</param>
        /// <param name="label">The memory allocator label to use.</param>
        /// <param name="leaveInputOpen">True to leave the input stream open after the reader object is disposed; otherwise, false.</param>
        /// <param name="leaveOutputOpen">True to leave the output stream open after the reader object is disposed; otherwise, false.</param>
        /// <exception cref="ArgumentException">The configuration is invalid.</exception>
        public SerializedObjectReader(Stream input, PackedBinaryStream output, SerializedObjectReaderConfiguration configuration, Allocator label = SerializationConfiguration.DefaultAllocatorLabel, bool leaveInputOpen = true, bool leaveOutputOpen = true)
        {
            m_Label = label;
            m_Data = (UnsafeSerializedObjectReader*) UnsafeUtility.Malloc(sizeof(UnsafeSerializedObjectReader), UnsafeUtility.AlignOf<UnsafeSerializedObjectReader>(), m_Label);
            *m_Data = new UnsafeSerializedObjectReader(input, output, configuration, label, leaveInputOpen, leaveOutputOpen);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedObjectReader"/> class based on the specified input buffer and configuration.
        /// </summary>
        /// <param name="buffer">The pointer to the input buffer.</param>
        /// <param name="length">The input buffer length.</param>
        /// <param name="configuration">The configuration parameters to use for the reader.</param>
        /// <param name="label">The memory allocator label to use.</param>
        public SerializedObjectReader(char* buffer, int length, SerializedObjectReaderConfiguration configuration, Allocator label = SerializationConfiguration.DefaultAllocatorLabel)
            : this(buffer, length, OpenBinaryStreamWithConfiguration(configuration, label), configuration, label, false)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedObjectReader"/> class based on the specified input buffer, output stream and configuration.
        /// </summary>
        /// <param name="buffer">The pointer to the input buffer.</param>
        /// <param name="length">The input buffer length.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="configuration">The configuration parameters to use for the reader.</param>
        /// <param name="label">The memory allocator label to use.</param>
        public SerializedObjectReader(char* buffer, int length, PackedBinaryStream output, SerializedObjectReaderConfiguration configuration, Allocator label = SerializationConfiguration.DefaultAllocatorLabel)
            : this(buffer, length, output, configuration, label, true)
        {
        }

        internal SerializedObjectReader(char* buffer, int length, PackedBinaryStream output, SerializedObjectReaderConfiguration configuration, Allocator label = SerializationConfiguration.DefaultAllocatorLabel, bool leaveOutputOpen = true)
        {
            m_Label = label;
            m_Data = (UnsafeSerializedObjectReader*) UnsafeUtility.Malloc(sizeof(UnsafeSerializedObjectReader), UnsafeUtility.AlignOf<UnsafeSerializedObjectReader>(), m_Label);
            *m_Data = new UnsafeSerializedObjectReader(buffer, length, output, configuration, label, leaveOutputOpen);
        }

        /// <summary>
        /// Releases all resources used by the <see cref="SerializedObjectReader" />.
        /// </summary>
        public void Dispose()
        {
            m_Data->Dispose();
            UnsafeUtility.Free(m_Data, m_Label);
            m_Data = null;
        }

        /// <summary>
        /// Resets the reader for re-use.
        /// </summary>
        public void Reset()
        {
            m_Data->Reset();
        }

        /// <summary>
        /// Sets the data source for the reader.
        /// </summary>
        /// <remarks>
        /// This will invalidate any generated views from this reader.
        /// </remarks>
        /// <param name="ptr">The char buffer.</param>
        /// <param name="length">The char buffer length.</param>
        public void SetSource(char* ptr, int length)
        {
            m_Data->SetSource(ptr, length);
        }

        /// <summary>
        /// Advances the reader to the given node type, ignoring depth/scope.
        /// </summary>
        /// <param name="node">The node type to stop at.</param>
        /// <returns>The node type the parser stopped at.</returns>
        public NodeType Step(NodeType node = NodeType.Any)
        {
            return m_Data->Step(node);
        }

        /// <summary>
        /// Advances the reader to the given node type, ignoring depth/scope.
        /// </summary>
        /// <param name="view">The view at the returned node type.</param>
        /// <param name="node">The node type to stop at.</param>
        /// <returns>The node type the parser stopped at.</returns>
        public NodeType Step(out SerializedValueView view, NodeType node = NodeType.Any)
        {
            return m_Data->Step(out view, node);
        }
        
        /// <summary>
        /// Reads the specified data and returns a new <see cref="SerializedValueView"/>.
        /// </summary>
        /// <remarks>
        /// This will invalidate any generated views from this reader.
        /// </remarks>
        /// <param name="ptr">The char buffer.</param>
        /// <param name="length">The char buffer length.</param>
        /// <returns>The view for the first node in the stream.</returns>
        public SerializedValueView Read(char* ptr, int length)
        {
            SetSource(ptr, length);
            m_Data->Read(out var view);
            return view;
        }

        /// <summary>
        /// Reads the next node in the stream, respecting depth/scope.
        /// </summary>
        /// <param name="node">The node type to stop at.</param>
        /// <returns>The node type that was read.</returns>
        public NodeType Read(NodeType node = NodeType.Any)
        {
            return m_Data->Read(node);
        }

        /// <summary>
        /// Reads the next node in the stream, respecting depth/scope.
        /// </summary>
        /// <param name="view">The view at the returned node type.</param>
        /// <param name="node">The node type to stop at.</param>
        /// <returns>The node type the parser stopped at.</returns>
        public NodeType Read(out SerializedValueView view, NodeType node = NodeType.Any)
        {
            return m_Data->Read(out view, node);
        }

        /// <summary>
        /// Reads the next node as a <see cref="SerializedObjectView"/>
        /// </summary>
        /// <returns>The <see cref="SerializedObjectView"/> that was read.</returns>
        /// <exception cref="InvalidOperationException">The reader state is invalid.</exception>
        public SerializedObjectView ReadObject()
        {
            return m_Data->ReadObject();
        }

        /// <summary>
        /// Reads the next node as a <see cref="SerializedMemberView"/>.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">The reader state is invalid.</exception>
        public SerializedMemberView ReadMember()
        {
            return m_Data->ReadMember();
        }

        /// <summary>
        /// Reads the next node as a member, respecting depth/scope and adds it to the given <see cref="SerializedMemberViewCollection"/>.
        /// </summary>
        /// <param name="collection">The collection to add the member to.</param>
        public void ReadMember(SerializedMemberViewCollection collection)
        {
            m_Data->ReadMember(collection);
        }

        /// <summary>
        /// Reads the next node as an array element.
        /// </summary>
        /// <param name="view">The view of the array element.</param>
        /// <returns>True if the element was successfully read, false otherwise.</returns>
        public bool ReadArrayElement(out SerializedValueView view)
        {
            return m_Data->ReadArrayElement(out view);
        }

        /// <summary>
        /// Reads the next <see cref="count"/> elements of an array and writes views to the given buffer.
        /// </summary>
        /// <param name="views">The array to write the views to.</param>
        /// <param name="count">The number of elements to read.</param>
        /// <returns>The number of elements read.</returns>
        /// <exception cref="IndexOutOfRangeException">The count exceeded the array of views.</exception>
        public int ReadArrayElementBatch(NativeArray<SerializedValueView> views, int count)
        {
            if (count > views.Length)
                throw new IndexOutOfRangeException();
            
            return m_Data->ReadArrayElementBatch((SerializedValueView*) views.GetUnsafePtr(), count);
        }

        /// <summary>
        /// Reads the next <see cref="count"/> elements of an array and writes views to the given buffer.
        /// </summary>
        /// <param name="views">The buffer to write the views to.</param>
        /// <param name="count">The number of elements to read.</param>
        /// <returns>The number of elements read.</returns>
        public int ReadArrayElementBatch(SerializedValueView* views, int count)
        {
            return m_Data->ReadArrayElementBatch(views, count);
        }
        
        /// <summary>
        /// Discards completed data from the buffers.
        /// </summary>
        public void DiscardCompleted()
        {
            m_Data->DiscardCompleted();
        }

        /// <summary>
        /// Throws any invalid json exceptions generated from within a burst context.
        /// </summary>
        public void CheckAndThrowInvalidJsonException()
        {
            m_Data->CheckAndThrowInvalidJsonException();
        }
    }
}