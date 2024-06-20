using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using Unity.Serialization.Json.Unsafe;

namespace Unity.Serialization.Json
{
    /// <summary>
    /// The <see cref="IJsonSerializationContext"/> provides an untyped context for contravariant serialization adapters.
    /// </summary>
    public interface IJsonSerializationContext
    {
        /// <summary>
        /// Gets the underlying <see cref="JsonWriter"/> which can be used to output formatted data.
        /// </summary>
        JsonWriter Writer { get; }

        /// <summary>
        /// Continues serialization for the current value. This will run the next adapter in the sequence, or the default behaviour.
        /// </summary>
        void ContinueVisitation();

        /// <summary>
        /// Continues serialization for the current type without running any more adapters. This will perform the default behaviour.
        /// </summary>
        void ContinueVisitationWithoutAdapters();
        
        /// <summary>
        /// Writes the given <paramref name="value"/> to the output using all adapters.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <typeparam name="T">The value type to write.</typeparam>
        void SerializeValue<T>(T value);
        
        /// <summary>
        /// Writes the given <paramref name="key"/>-<paramref name="value"/> pair to the output. This will run all adapters.
        /// </summary>
        /// <param name="key">The key to write.</param>
        /// <param name="value">The value to write.</param>
        /// <typeparam name="T">The value type to write.</typeparam>
        void SerializeValue<T>(string key, T value);
    }

    /// <summary>
    /// The <see cref="JsonSerializationContext{T}"/> is available from adapters. It provides access to the current adapter enumerator and allows for control of serialization for a given type. 
    /// </summary>
    /// <typeparam name="TValue">The value type being serialized.</typeparam>
    public readonly struct JsonSerializationContext<TValue> : IJsonSerializationContext
    {
        static readonly AdapterProperty k_AdapterProperty = new AdapterProperty();
        
        class AdapterProperty : IProperty
        {
            public Type DeclaredType { get; set; }
            
            public string Name => string.Empty;
            public bool IsReadOnly => true;
            public Type DeclaredValueType() => DeclaredType;
            public bool HasAttribute<TAttribute>() where TAttribute : Attribute => false;
            public TAttribute GetAttribute<TAttribute>() where TAttribute : Attribute => default;
            public IEnumerable<TAttribute> GetAttributes<TAttribute>() where TAttribute : Attribute => Enumerable.Empty<TAttribute>();
            public IEnumerable<Attribute> GetAttributes() => Enumerable.Empty<Attribute>();
        }

        readonly JsonPropertyWriter m_Visitor;
        readonly JsonAdapterCollection.Enumerator m_Adapters;
        readonly TValue m_Value;
        readonly bool m_IsRoot;

        /// <summary>
        /// Gets the underlying <see cref="JsonWriter"/> which can be used to output formatted data.
        /// </summary>
        public JsonWriter Writer => m_Visitor.Writer;

        internal JsonSerializationContext(JsonPropertyWriter visitor, JsonAdapterCollection.Enumerator adapters, TValue value, bool isRoot)
        {
            m_Visitor = visitor;
            m_Adapters = adapters;
            m_Value = value;
            m_IsRoot = isRoot;
        }

        /// <summary>
        /// Continues visitation for the current type. This will run the next adapter in the sequence, or the default behaviour.
        /// </summary>
        public void ContinueVisitation()
            => m_Visitor.WriteValueWithAdapters(m_Value, m_Adapters, m_IsRoot);

        /// <summary>
        /// Continues visitation for the current type without running any more adapters. This will perform the default behaviour.
        /// </summary>
        public void ContinueVisitationWithoutAdapters()
            => m_Visitor.WriteValueWithoutAdapters(m_Value, m_IsRoot);
        
        /// <summary>
        /// Writes the given <paramref name="value"/> to the output. This will run all adapters.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <typeparam name="T">The value type to write.</typeparam>
        public void SerializeValue<T>(T value)
        {
            k_AdapterProperty.DeclaredType = typeof(T);
            
            using (m_Visitor.CreatePropertyScope(k_AdapterProperty))
                m_Visitor.WriteValue(ref value);
        }

        /// <summary>
        /// Writes the given <paramref name="key"/>-<paramref name="value"/> pair to the output. This will run all adapters.
        /// </summary>
        /// <param name="key">The key to write.</param>
        /// <param name="value">The value to write.</param>
        /// <typeparam name="T">The value type to write.</typeparam>
        public void SerializeValue<T>(string key, T value)
        {
            k_AdapterProperty.DeclaredType = typeof(T);
            
            using (m_Visitor.CreatePropertyScope(k_AdapterProperty))
            {
                m_Visitor.Writer.WriteKey(key);
                m_Visitor.WriteValue(ref value);
            }
        }
    }

    /// <summary>
    /// The <see cref="IJsonDeserializationContext"/> provides an untyped context for contravariant serialization adapters.
    /// </summary>
    public interface IJsonDeserializationContext
    {
        /// <summary>
        /// Gets the serialized view for value being deserialized. 
        /// </summary>
        SerializedValueView SerializedValue { get; }

        /// <summary>
        /// Gets the existing instance if overwriting; otherwise default.
        /// </summary>
        /// <returns>The existing instance being deserialized in to, or default.</returns>
        object GetInstance();

        /// <summary>
        /// Continues de-serialization for the current value. This will run the next adapter in the sequence, or the default behaviour.
        /// </summary>
        /// <returns>The deserialized value.</returns>
        object ContinueVisitation();
        
        /// <summary>
        /// Continues de-serialization for the current type without running any more adapters. This will perform the default behaviour.
        /// </summary>
        /// <returns>The deserialized value.</returns>
        object ContinueVisitationWithoutAdapters();

        /// <summary>
        /// Reads the given value type from the stream.
        /// </summary>
        /// <param name="view">The view containing the serialized data.</param>
        /// <typeparam name="T">The value type to deserialize.</typeparam>
        /// <returns>The deserialized value.</returns>
        T DeserializeValue<T>(SerializedValueView view);
    }

    /// <summary>
    /// The <see cref="JsonDeserializationContext{T}"/> is available from adapters. It provides access to the current adapter enumerator and allows for control of deserialization for a given type. 
    /// </summary>
    /// <typeparam name="TValue">The value type being deserialized.</typeparam>
    public readonly struct JsonDeserializationContext<TValue> : IJsonDeserializationContext
    {
        readonly JsonPropertyReader m_Visitor;
        readonly JsonAdapterCollection.Enumerator m_Adapters;
        readonly TValue m_Value;
        readonly UnsafeValueView m_View;
        readonly bool m_IsRoot;
        
        /// <summary>
        /// The in-memory representation of the value being deserialized.
        /// </summary>
        public SerializedValueView SerializedValue => m_View.AsSafe();

        internal JsonDeserializationContext(JsonPropertyReader visitor, JsonAdapterCollection.Enumerator adapters, TValue value, UnsafeValueView view, bool isRoot)
        {
            m_Visitor = visitor;
            m_Adapters = adapters;
            m_Value = value;
            m_View = view;
            m_IsRoot = isRoot;
        }
        
        /// <inheritdoc/>
        object IJsonDeserializationContext.GetInstance() => GetInstance();
        
        /// <summary>
        /// Gets the existing instance if overwriting; otherwise default.
        /// </summary>
        /// <returns>The existing instance of <see cref="TValue"/> or default.</returns>
        public TValue GetInstance() => m_Value;
        
        /// <summary>
        /// Continues visitation for the current type. This will run the next adapter in the sequence, or the default behaviour and return the deserialized value.
        /// </summary>
        /// <returns>The deserialized value.</returns>
        public TValue ContinueVisitation()
        {
            var value = default(TValue);
            m_Visitor.ReadValueWithAdapters(ref value, m_View, m_Adapters, m_IsRoot);
            return value;
        }
        
        /// <summary>
        /// Continues visitation for the current type using the specified <see cref="SerializedValueView"/>. This will run the next adapter in the sequence, or the default behaviour and return the deserialized value.
        /// </summary>
        /// <param name="view">A view on the serialized data.</param>
        /// <returns>The deserialized value.</returns>
        public TValue ContinueVisitation(SerializedValueView view)
        {
            var value = default(TValue);
            m_Visitor.ReadValueWithAdapters(ref value, view.AsUnsafe(), m_Adapters, m_IsRoot);
            return value;
        }

        /// <summary>
        /// Continues visitation for the current type. This will run the next adapter in the sequence, or the default behaviour and return the deserialized value.
        /// </summary>
        /// <param name="value">The value being deserialized.</param>
        public void ContinueVisitation(ref TValue value)
        {
            m_Visitor.ReadValueWithAdapters(ref value, m_View, m_Adapters, m_IsRoot);
        }
        
        /// <summary>
        /// Continues visitation for the current type using the specified <see cref="SerializedValueView"/>. This will run the next adapter in the sequence, or the default behaviour and return the deserialized value.
        /// </summary>
        /// <param name="value">The value being deserialized.</param>
        /// <param name="view">A view on the serialized data.</param>
        public void ContinueVisitation(ref TValue value, SerializedValueView view)
        {
            m_Visitor.ReadValueWithAdapters(ref value, view.AsUnsafe(), m_Adapters, m_IsRoot);
        }
        
        /// <summary>
        /// Continues visitation for the current type. This will run the next adapter in the sequence, or the default behaviour and return the deserialized value.
        /// </summary>
        /// <returns>The deserialized value.</returns>
        public TValue ContinueVisitationWithoutAdapters()
        {
            var value = default(TValue);
            m_Visitor.ReadValueWithoutAdapters(ref value, m_View, m_IsRoot);
            return value;
        }
        
        /// <summary>
        /// Continues visitation for the current type using the specified <see cref="SerializedValueView"/>. This will run the next adapter in the sequence, or the default behaviour and return the deserialized value.
        /// </summary>
        /// <param name="view">A view on the serialized data.</param>
        /// <returns>The deserialized value.</returns>
        public TValue ContinueVisitationWithoutAdapters(SerializedValueView view)
        {
            var value = default(TValue);
            m_Visitor.ReadValueWithoutAdapters(ref value, view.AsUnsafe(), m_IsRoot);
            return value;
        }

        /// <summary>
        /// Continues visitation for the current type. This will invoke the default behaviour and return the deserialized value.
        /// </summary>
        /// <param name="value">The value being deserialized.</param>
        public void ContinueVisitationWithoutAdapters(ref TValue value)
        {
            m_Visitor.ReadValueWithoutAdapters(ref value, m_View, m_IsRoot);
        }
        
        /// <summary>
        /// Continues visitation for the current type using the specified <see cref="SerializedValueView"/>. This will invoke the default behaviour and return the deserialized value..
        /// </summary>
        /// <param name="value">The value being deserialized.</param>
        /// <param name="view">A view on the serialized data.</param>
        public void ContinueVisitationWithoutAdapters(ref TValue value, SerializedValueView view)
        {
            m_Visitor.ReadValueWithoutAdapters(ref value, view.AsUnsafe(), m_IsRoot);
        }

        /// <inheritdoc cref="IJsonDeserializationContext.ContinueVisitation"/>
        object IJsonDeserializationContext.ContinueVisitation() => ContinueVisitation();
        
        /// <inheritdoc cref="IJsonDeserializationContext.ContinueVisitationWithoutAdapters"/>
        object IJsonDeserializationContext.ContinueVisitationWithoutAdapters() => ContinueVisitationWithoutAdapters();
        
        /// <summary>
        /// Reads the given <see cref="SerializedValue"/> as <typeparamref name="T"/> and returns it. This will run all adapters.
        /// </summary>
        /// <param name="view">A view on the serialized data.</param>
        /// <typeparam name="T">The type of the value being deserialized.</typeparam>
        /// <returns>The deserialized value.</returns>
        public T DeserializeValue<T>(SerializedValueView view)
        {
            var value = default(T);
            m_Visitor.ReadValue(ref value, view.AsUnsafe() );
            return value;
        }
    }
}
