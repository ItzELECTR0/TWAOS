using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Serialization.Json.Unsafe;

namespace Unity.Serialization.Json
{
    /// <summary>
    /// A view on top of the <see cref="PackedBinaryStream"/> that represents an array of values.
    /// </summary>
    public readonly unsafe struct SerializedArrayView : ISerializedView, IList<SerializedValueView>
    {        
        static SerializedArrayView()
        {
            Properties.PropertyBag.Register(new SerializedArrayViewPropertyBag());
        }
        
        /// <summary>
        /// Enumerates the elements of <see cref="SerializedArrayView"/>.
        /// </summary>
        public struct Enumerator : IEnumerator<SerializedValueView>
        {
            readonly UnsafePackedBinaryStream* m_Stream;
            readonly Handle m_Start;
            Handle m_Current;

            internal Enumerator(UnsafePackedBinaryStream* stream, Handle start)
            {
                m_Stream = stream;
                m_Start = start;
                m_Current = new Handle {Index = -1, Version = -1};
            }

            /// <summary>
            /// Advances the enumerator to the next element of the <see cref="SerializedArrayView"/>.
            /// </summary>
            /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext()
            {
                var startIndex = m_Stream->GetTokenIndex(m_Start);
                var startToken = m_Stream->GetToken(startIndex);

                if (startToken.Length == 1)
                {
                    return false;
                }

                if (m_Current.Index == -1)
                {
                    m_Current = m_Stream->GetFirstChild(m_Start);
                    return true;
                }

                if (!m_Stream->IsValid(m_Current))
                {
                    return false;
                }
                
                var currentIndex = m_Stream->GetTokenIndex(m_Current);
                var currentToken = m_Stream->GetToken(currentIndex);

                if (currentIndex + currentToken.Length >= startIndex + startToken.Length)
                {
                    return false;
                }
                
                m_Current = m_Stream->GetHandle(currentIndex + currentToken.Length);
                return true;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                m_Current = new Handle {Index = -1, Version = -1};
            }

            /// <summary>
            /// The element in the <see cref="SerializedArrayView"/> at the current position of the enumerator.
            /// </summary>
            /// <exception cref="InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.</exception>
            public SerializedValueView Current
            {
                get
                {
                    if (m_Current.Index < 0)
                    {
                        throw new InvalidOperationException();
                    }
                    return new SerializedValueView(m_Stream, m_Current);
                }
            }

            object IEnumerator.Current => Current;

            /// <summary>
            /// Releases all resources used by the <see cref="SerializedArrayView.Enumerator" />.
            /// </summary>
            public void Dispose()
            {
            }
        }
        
        [NativeDisableUnsafePtrRestriction] readonly UnsafePackedBinaryStream* m_Stream;
        readonly Handle m_Handle;
      
        internal SerializedArrayView(UnsafePackedBinaryStream* stream, Handle handle)
        {
            m_Stream = stream;
            m_Handle = handle;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="SerializedArrayView"/>.
        /// </summary>
        /// <returns>A <see cref="SerializedArrayView.Enumerator"/> for the <see cref="SerializedArrayView"/>.</returns>
        public Enumerator GetEnumerator() => new Enumerator(m_Stream, m_Handle);

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="SerializedArrayView"/>.
        /// </summary>
        /// <returns>A <see cref="SerializedArrayView.Enumerator"/> for the <see cref="SerializedArrayView"/>.</returns>
        IEnumerator<SerializedValueView> IEnumerable<SerializedValueView>.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="SerializedArrayView"/>.
        /// </summary>
        /// <returns>A <see cref="SerializedArrayView.Enumerator"/> for the <see cref="SerializedArrayView"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        bool ICollection<SerializedValueView>.IsReadOnly => true;
        
        SerializedValueView IList<SerializedValueView>.this[int index]
        {
            get
            {
                using (var enumerator = GetEnumerator())
                {
                    var i = -1;
                    
                    while (enumerator.MoveNext())
                    {
                        i++;

                        if (i == index)
                            return enumerator.Current;
                    }
                }

                throw new IndexOutOfRangeException();
            }
            set => throw new NotSupportedException($"{nameof(SerializedArrayView)} is immutable");
        }

        int ICollection<SerializedValueView>.Count
        {
            get
            {
                var index = m_Stream->GetTokenIndex(m_Handle);
                var token = m_Stream->GetToken(index);

                if (token.Length <= 1)
                    return 0;
                
                var count = 0;
                var childHandle = m_Stream->GetFirstChild(m_Handle);

                for (;;)
                {
                    if (!m_Stream->IsValid(childHandle))
                        return count;

                    count++;
                    
                    var childIndex = m_Stream->GetTokenIndex(childHandle);
                    var childToken = m_Stream->GetToken(childIndex);
                    
                    if (childIndex + childToken.Length >= index + token.Length)
                        return count;
                    
                    childHandle = m_Stream->GetHandle(childIndex + childToken.Length);
                }
            }
        }

        /// <inheritdoc cref="ICollection{T}.Contains"/>
        bool ICollection<SerializedValueView>.Contains(SerializedValueView item)
        {
            if (item.m_Stream != m_Stream)
                return false;
            
            using (var enumerator = GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (item.m_Handle.Equals(enumerator.Current.m_Handle))
                        return true;
                }
            }

            return false;
        }

        /// <inheritdoc cref="ICollection{T}.CopyTo"/>
        void ICollection<SerializedValueView>.CopyTo(SerializedValueView[] array, int arrayIndex)
        {
            using (var enumerator = GetEnumerator())
            {
                while (enumerator.MoveNext())
                    array[arrayIndex++] = enumerator.Current;
            }
        }

        /// <inheritdoc cref="IList{T}.IndexOf"/>
        int IList<SerializedValueView>.IndexOf(SerializedValueView item)
        {
            var index = -1;

            if (item.m_Stream != m_Stream)
                return index;
            
            using (var enumerator = GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    index++;
                    
                    if (item.m_Handle.Equals(enumerator.Current.m_Handle))
                        return index;
                }
            }

            return -1;
        }

        /// <inheritdoc cref="ICollection{T}.Clear"/>
        void ICollection<SerializedValueView>.Clear()
            => throw new NotSupportedException($"{nameof(SerializedArrayView)} is immutable");
        
        /// <inheritdoc cref="IList{T}.Insert"/>
        void IList<SerializedValueView>.Insert(int index, SerializedValueView item)
            => throw new NotSupportedException($"{nameof(SerializedArrayView)} is immutable");
        
        /// <inheritdoc cref="ICollection{T}.Add"/>
        void ICollection<SerializedValueView>.Add(SerializedValueView item)
            => throw new NotSupportedException($"{nameof(SerializedArrayView)} is immutable");

        /// <inheritdoc cref="ICollection{T}.Remove"/>
        bool ICollection<SerializedValueView>.Remove(SerializedValueView item)
            => throw new NotSupportedException($"{nameof(SerializedArrayView)} is immutable");

        /// <inheritdoc cref="IList{T}.RemoveAt"/>
        void IList<SerializedValueView>.RemoveAt(int index)
            => throw new NotSupportedException($"{nameof(SerializedArrayView)} is immutable");
        
        internal UnsafeArrayView AsUnsafe() => new UnsafeArrayView(m_Stream, m_Stream->GetTokenIndex(m_Handle));
    }
}