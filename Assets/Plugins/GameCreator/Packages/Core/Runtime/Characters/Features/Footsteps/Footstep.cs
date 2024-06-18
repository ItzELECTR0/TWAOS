using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class Footstep : TPolymorphicItem<Footstep>
    {
        [SerializeField] private Bone m_Bone;

        // PROPERTIES: ----------------------------------------------------------------------------

        public Bone Bone => this.m_Bone;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public Footstep()
        { }

        public Footstep(HumanBodyBones bone)
        {
            this.m_Bone = new Bone(bone);
        }

        public Footstep(string bonePath)
        {
            this.m_Bone = new Bone(bonePath);
        }
    }
}