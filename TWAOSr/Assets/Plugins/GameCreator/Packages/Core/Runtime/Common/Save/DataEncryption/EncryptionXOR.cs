using System;
using System.Text;
using UnityEngine;

namespace GameCreator.Runtime.Common.SaveSystem
{
    [Title("XOR")]
    [Category("XOR")]
    
    [Image(typeof(IconOR), ColorTheme.Type.Blue)]
    [Description("Uses a XOR operator to hide values")]
    
    [Serializable]
    public class EncryptionXOR : TDataEncryption
    {
        [SerializeField] private string m_Secret = "Colloportus";
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public override string Encrypt(string input) => this.XOR(input);
        public override string Decrypt(string input) => this.XOR(input);

        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private string XOR(string input)
        {
            StringBuilder output = new StringBuilder();
            for (int i = 0; i < input.Length; ++i)
            {
                int secretIndex = i % this.m_Secret.Length;
                int xor = input[i] ^ this.m_Secret[secretIndex];
                
                output.Append((char) xor);
            }
            
            return output.ToString();
        }
    }
}