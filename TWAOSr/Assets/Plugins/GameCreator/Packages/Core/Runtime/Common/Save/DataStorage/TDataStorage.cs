using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GameCreator.Runtime.Common.SaveSystem
{
    [Title("Save System")]
    
    [Serializable]
    public abstract class TDataStorage : IDataStorage
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private IDataEncryption m_Cryptography;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected IDataEncryption Cryptography
        {
            get
            {
                if (this.m_Cryptography == null)
                {
                    Debug.LogError("No Encryption system provided");
                    this.m_Cryptography = new EncryptionNone();
                }
                
                return this.m_Cryptography;
            }
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public Task WithEncryption(IDataEncryption encryption)
        {
            this.m_Cryptography = encryption;
            return Task.FromResult(1);
        }
        
        // ABSTRACT METHODS: ----------------------------------------------------------------------
        
        public abstract Task Commit();
        
        public abstract Task DeleteAll();
        public abstract Task DeleteKey(string key);
        
        public abstract Task<bool> HasKey(string key);
        
        public abstract Task<object> Get(string key, Type type);
        public abstract Task Set(string key, object value);
    }
}