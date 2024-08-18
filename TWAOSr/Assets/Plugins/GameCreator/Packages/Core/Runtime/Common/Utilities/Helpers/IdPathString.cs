using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public struct IdPathString : ISerializationCallbackReceiver
    {
        [SerializeField] private string m_String;
        [SerializeField] private int m_ID;

        // PROPERTIES: ----------------------------------------------------------------------------

        public string String => this.m_String;
        public int ID => this.m_ID;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public IdPathString(string value)
        {
            this.m_String = value;
            this.m_ID = new PropertyName(value).GetHashCode();
        }
        
        // SERIALIZATION: -------------------------------------------------------------------------

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (AssemblyUtils.IsReloading) return;
            if (string.IsNullOrEmpty(this.m_String)) return;
            
            this.m_String = TextUtils.ProcessID(this.m_String, true);
            this.m_ID = new PropertyName(this.m_String).GetHashCode();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        { }
    }
}