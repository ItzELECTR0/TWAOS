using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class Tokens
    {
        [SerializeReference]
        private Token[] m_Tokens = Array.Empty<Token>();

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public Tokens()
        { }

        public Tokens(GameObject target, Memory[] memories) : this()
        {
            List<Token> tokens = new List<Token>();
            foreach (Memory memory in memories)
            {
                if (!memory.IsEnabled) continue;
                tokens.Add(memory.GetToken(target));
            }

            this.m_Tokens = tokens.ToArray();
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Token Get(int i)
        {
            if (i < 0) return null;
            if (i >= this.m_Tokens.Length) return null;

            return this.m_Tokens[i];
        }
    }
}