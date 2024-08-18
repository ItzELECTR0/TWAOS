using System;

namespace GameCreator.Runtime.Common.SaveSystem
{
    [Title("None")]
    [Category("None")]
    
    [Image(typeof(IconEmpty), ColorTheme.Type.TextLight)]
    [Description("Does not use any type of encryption")]
    
    [Serializable]
    public class EncryptionNone : TDataEncryption
    {
        public override string Encrypt(string input) => input;
        public override string Decrypt(string input) => input;
    }
}