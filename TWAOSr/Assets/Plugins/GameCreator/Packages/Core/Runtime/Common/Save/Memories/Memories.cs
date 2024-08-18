using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class Memories : TPolymorphicList<Memory>
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeReference] private Memory[] m_Memories =
        {
            new MemoryPosition(),
            new MemoryRotation(),
            new MemoryScale(),
        };
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override int Length => this.m_Memories.Length;

        public Type SaveType => typeof(Tokens);

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Tokens GetTokens(GameObject target)
        {
            return new Tokens(target, this.m_Memories);
        }

        public void OnRemember(GameObject target, Tokens tokens)
        {
            if (tokens == null) return;
            
            for (int i = 0; i < this.Length; ++i)
            {
                Token token = tokens.Get(i);
                if (!this.m_Memories[i].IsEnabled) continue;
                
                this.m_Memories[i].OnRemember(target, token);
            }
        }
    }
}