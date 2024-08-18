using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public struct IdString : ISerializationCallbackReceiver, IEquatable<IdString>
    {
        public static readonly IdString EMPTY = new IdString(string.Empty);
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private string m_String;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private int m_Hash;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public string String
        {
            get => this.m_String ?? string.Empty;
            set
            {
                this.m_String = value;
                this.m_Hash = value.GetHashCode();
            }
        }

        public int Hash
        {
            get
            {
                if (this.m_Hash == 0) this.m_Hash = this.String.GetHashCode();
                return this.m_Hash;
            }
        }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public IdString(string value)
        {
            this.m_String = value;
            this.m_Hash = 0;
        }
        
        // SERIALIZATION: -------------------------------------------------------------------------

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (AssemblyUtils.IsReloading) return;
            if (string.IsNullOrEmpty(this.m_String)) return;
            
            this.m_String = TextUtils.ProcessID(this.m_String, false);
            this.m_Hash = this.m_String.GetHashCode();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.m_Hash = this.m_String.GetHashCode();
        }
        
        // OVERRIDES: -----------------------------------------------------------------------------

        public override int GetHashCode() => this.Hash;

        public bool Equals(IdString other)
        {
            return this.Hash == other.Hash;
        }

        public override bool Equals(object other)
        {
            return other is IdString otherIdString && this.Equals(otherIdString);
        }

        public override string ToString()
        {
            return this.m_String;
        }
        
        // OPERATORS: -----------------------------------------------------------------------------
        
        public static bool operator ==(IdString left, IdString right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IdString left, IdString right)
        {
            return !left.Equals(right);
        }
    }
}