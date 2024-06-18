using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class ReactionList : TPolymorphicList<ReactionItem>
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeReference] private ReactionItem[] m_List = Array.Empty<ReactionItem>();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override int Length => this.m_List.Length;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public ReactionList()
        { }

        public ReactionList(params ReactionItem[] reactions) : this()
        {
            this.m_List = reactions;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public ReactionItem Get(Args args, Vector3 direction, float power)
        {
            foreach (ReactionItem candidate in this.m_List)
            {
                if (!candidate.CheckDirection(direction)) continue;
                if (!candidate.CheckPower(power)) continue;
                
                if (!candidate.CheckConditions(args)) continue;
                return candidate;
            }

            return null;
        }
    }
}