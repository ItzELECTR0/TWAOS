using System;
using System.Text;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Random")]
    [Category("Random/Random")]
    
    [Image(typeof(IconDice), ColorTheme.Type.Yellow)]
    [Description("Returns a random string of a certain length and particular set of characters")]
    
    [Serializable]
    public class GetStringRandom : PropertyTypeGetString
    {
        private static readonly string ALPHABET = "abcdefghijklmnopqrstuvwxyz";
        private static readonly string NUMBERS = "0123456789";
        private static readonly string SYMBOLS = "!#$%&'()*+,-./:<=>?@[]^{}|~";
        
        public enum Type
        {
            Numbers,
            Alphabet,
            AlphaNumeric,
            AlphanumericSymbolic
        }
        
        [SerializeField] private int m_Length = 8;
        [SerializeField] private Type m_Type = Type.AlphaNumeric;

        public override string Get(Args args) => Generate(this.m_Length, this.m_Type);
        
        public override string Get(GameObject gameObject) => Generate(this.m_Length, this.m_Type);

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringRandom()
        );
        
        private static char GenerateAlphabet => ALPHABET[UnityEngine.Random.Range(0, ALPHABET.Length)];
        private static char GenerateNumber => NUMBERS[UnityEngine.Random.Range(0, NUMBERS.Length)];
        private static char GenerateSymbol => SYMBOLS[UnityEngine.Random.Range(0, SYMBOLS.Length)];

        private static string Generate(int length, Type type)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < length; ++i)
            {
                stringBuilder.Append(type switch
                {
                    Type.Numbers => GenerateNumber,
                    Type.Alphabet => GenerateAlphabet,
                    Type.AlphaNumeric => UnityEngine.Random.Range(0, 2) switch
                    {
                        0 => GenerateAlphabet,
                        _ => GenerateNumber
                    },
                    Type.AlphanumericSymbolic => UnityEngine.Random.Range(0, 3) switch
                    {
                        0 => GenerateAlphabet,
                        1 => GenerateNumber,
                        _ => GenerateSymbol
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                });
            }
            
            return stringBuilder.ToString();
        }

        public override string String => "Random";
    }
}