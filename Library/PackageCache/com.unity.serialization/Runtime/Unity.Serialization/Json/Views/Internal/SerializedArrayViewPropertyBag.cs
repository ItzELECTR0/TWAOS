using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;

namespace Unity.Serialization.Json
{
    class SerializedArrayViewPropertyBag : PropertyBag<SerializedArrayView>, IListPropertyBag<SerializedArrayView, SerializedValueView>
    {
        static readonly Property k_Property = new Property();

        class Property : Property<SerializedArrayView, SerializedValueView>, IListElementProperty
        {
            // ReSharper disable once InconsistentNaming
            internal int m_Index;
            
            // ReSharper disable once InconsistentNaming
            internal SerializedValueView m_View;
            
            /// <inheritdoc/>
            public int Index => m_Index;

            /// <inheritdoc/>
            public override string Name => m_Index.ToString();
            
            /// <inheritdoc/>
            public override bool IsReadOnly => true;

            public override SerializedValueView GetValue(ref SerializedArrayView container)
                => m_View;

            public override void SetValue(ref SerializedArrayView container, SerializedValueView value)
                => throw new InvalidOperationException("Property is ReadOnly.");
        }
        
        struct Enumerator : IEnumerator<IProperty<SerializedArrayView>>
        {
            int m_Index;
            
            SerializedArrayView.Enumerator m_Enumerator;

            public Enumerator(SerializedArrayView.Enumerator enumerator)
            {
                m_Index = -1;
                m_Enumerator = enumerator;
            }

            public bool MoveNext()
            {
                m_Index++;
                return m_Enumerator.MoveNext();  
            }

            public void Reset()
            {
                m_Index = -1;
                m_Enumerator.Reset();
            }

            public void Dispose()
                => m_Enumerator.Dispose();

            object IEnumerator.Current
                => Current;

            public IProperty<SerializedArrayView> Current
            {
                get
                {
                    k_Property.m_Index = m_Index;
                    k_Property.m_View = m_Enumerator.Current;
                    return k_Property;
                }
            }
        }

        readonly struct Enumerable : IEnumerable<IProperty<SerializedArrayView>>
        {
            readonly SerializedArrayView m_Container;

            public Enumerable(SerializedArrayView container) 
                => m_Container = container;

            public IEnumerator<IProperty<SerializedArrayView>> GetEnumerator()
                => new Enumerator(m_Container.GetEnumerator());

            IEnumerator IEnumerable.GetEnumerator()
                => new Enumerator(m_Container.GetEnumerator());
        }
        
        public override PropertyCollection<SerializedArrayView> GetProperties()
            => PropertyCollection<SerializedArrayView>.Empty;

        public override PropertyCollection<SerializedArrayView> GetProperties(ref SerializedArrayView container)
            => new PropertyCollection<SerializedArrayView>(new Enumerable(container));

        bool IIndexedProperties<SerializedArrayView>.TryGetProperty(ref SerializedArrayView container, int index, out IProperty<SerializedArrayView> property)
        {
            var enumerator = container.GetEnumerator();

            var i = -1;

            while (enumerator.MoveNext())
            {
                i++;

                if (i != index) 
                    continue;
                
                property = new Property {m_Index = index, m_View = enumerator.Current};
                return true;
            }
            
            property = null;
            return false;
        }

        void ICollectionPropertyBagAccept<SerializedArrayView>.Accept(ICollectionPropertyBagVisitor visitor, ref SerializedArrayView container)
        {
            visitor.Visit(this, ref container);
        }

        void IListPropertyBagAccept<SerializedArrayView>.Accept(IListPropertyBagVisitor visitor, ref SerializedArrayView container)
        {
            visitor.Visit(this, ref container);
        }

        void IListPropertyAccept<SerializedArrayView>.Accept<TContainer>(IListPropertyVisitor visitor, Property<TContainer, SerializedArrayView> property, ref TContainer container, ref SerializedArrayView list)
        {
            visitor.Visit<TContainer, SerializedArrayView, SerializedValueView>(property, ref container, ref list);
        }
    }
}