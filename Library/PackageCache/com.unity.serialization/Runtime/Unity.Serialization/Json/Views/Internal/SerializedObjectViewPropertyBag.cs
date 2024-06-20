using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;

namespace Unity.Serialization.Json
{
    class SerializedObjectViewPropertyBag : PropertyBag<SerializedObjectView>
    {
        static readonly Property k_Property = new Property();
        
        class Property : Property<SerializedObjectView, SerializedValueView>
        {
            // ReSharper disable once InconsistentNaming
            internal SerializedMemberView m_Member;

            /// <inheritdoc/>
            public override string Name => m_Member.Name().ToString();
            
            /// <inheritdoc/>
            public override bool IsReadOnly => true;

            /// <inheritdoc/>
            public override SerializedValueView GetValue(ref SerializedObjectView container)
                => m_Member.Value();

            /// <inheritdoc/>
            public override void SetValue(ref SerializedObjectView container, SerializedValueView value)
                => throw new InvalidOperationException("Property is ReadOnly.");
        }

        struct Enumerator : IEnumerator<IProperty<SerializedObjectView>>
        {
            SerializedObjectView.Enumerator m_Enumerator;
            
            public Enumerator(SerializedObjectView.Enumerator enumerator)
                => m_Enumerator = enumerator;

            public bool MoveNext()
                => m_Enumerator.MoveNext();

            public void Reset()
                => m_Enumerator.Reset();

            public void Dispose()
                => m_Enumerator.Dispose();

            object IEnumerator.Current 
                => Current;

            public IProperty<SerializedObjectView> Current
            {
                get
                {
                    var member = m_Enumerator.Current;
                    k_Property.m_Member = member;
                    return k_Property;
                }
            }
        }
        
        readonly struct Enumerable : IEnumerable<IProperty<SerializedObjectView>>
        {
            readonly SerializedObjectView m_Container;

            public Enumerable(SerializedObjectView container) 
                => m_Container = container;

            public IEnumerator<IProperty<SerializedObjectView>> GetEnumerator()
                => new Enumerator(m_Container.GetEnumerator());

            IEnumerator IEnumerable.GetEnumerator()
                => new Enumerator(m_Container.GetEnumerator());
        }
        
        public override PropertyCollection<SerializedObjectView> GetProperties()
            => PropertyCollection<SerializedObjectView>.Empty;

        public override PropertyCollection<SerializedObjectView> GetProperties(ref SerializedObjectView container)
            => new PropertyCollection<SerializedObjectView>(new Enumerable(container));
    }
}