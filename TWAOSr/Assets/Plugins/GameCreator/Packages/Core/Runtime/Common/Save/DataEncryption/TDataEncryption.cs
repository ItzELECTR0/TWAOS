using System;

namespace GameCreator.Runtime.Common.SaveSystem
{
    [Title("Encryption System")]
    
    [Serializable]
    public abstract class TDataEncryption : IDataEncryption
    {
        public abstract string Encrypt(string input);
        public abstract string Decrypt(string input);
    }
}