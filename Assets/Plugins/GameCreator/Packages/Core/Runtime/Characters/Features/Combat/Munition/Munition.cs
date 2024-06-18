using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class Munition : IMunition
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private int m_Id;
        [SerializeReference] private TMunitionValue m_Value;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public int Id => this.m_Id;
        public TMunitionValue Value => this.m_Value;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public Munition()
        { }

        public Munition(int id, TMunitionValue value)
        {
            this.m_Id = id;
            this.m_Value = value;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public object Clone()
        {
            return new Munition(
                this.m_Id,
                this.m_Value.Clone() as TMunitionValue
            );
        }
    }
}