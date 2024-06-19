using System;
using UnityEngine;

namespace Unity.Multiplayer.Editor
{
    [Serializable]
    internal struct SerializedType : ISerializationCallbackReceiver, IComparable<SerializedType>
    {
        [SerializeField] private string m_TypeName;
        private Type m_Type;

        public Type Value => m_Type;

        public SerializedType(Type type)
        {
            m_Type = type;
            m_TypeName = type.AssemblyQualifiedName;
        }

        public void OnBeforeSerialize()
        {
            m_TypeName = m_Type.AssemblyQualifiedName;
        }

        public void OnAfterDeserialize()
        {
            m_Type = Type.GetType(m_TypeName, false);
        }

        public int CompareTo(SerializedType other)
        {
            return m_TypeName.CompareTo(other.m_TypeName);
        }
    }
}
