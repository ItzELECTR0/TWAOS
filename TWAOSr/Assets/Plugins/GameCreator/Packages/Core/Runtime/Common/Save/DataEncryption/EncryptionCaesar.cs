using System;
using System.Text;
using UnityEngine;

namespace GameCreator.Runtime.Common.SaveSystem
{
    [Title("Caesar")]
    [Category("Caesar")]
    
    [Image(typeof(IconCrown), ColorTheme.Type.Yellow)]
    [Description("Uses a Caesar cipher that shifts character positions by N amount")]
    
    [Serializable]
    public class EncryptionCaesar : TDataEncryption
    {
        [SerializeField] private int m_Positions = 5;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public override string Encrypt(string input) => this.Caesar(input,  this.m_Positions);
        public override string Decrypt(string input) => this.Caesar(input, -this.m_Positions);
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        public string Caesar(string input, int positions)
        {
            StringBuilder output = new StringBuilder();
            
            foreach (char character in input)
            {
                int newCharPosition = character + positions;
                if (newCharPosition < 32 || newCharPosition > 126)
                {
                    output.Append(character);
                }
                else
                {
                    newCharPosition = (newCharPosition - 32) % 95 + 32;
                    output.Append((char) newCharPosition);
                }
            }

            return output.ToString();
        }
    }
}