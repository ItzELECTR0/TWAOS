using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters.IK
{
    [Serializable]
    public class LookSection
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private HumanBodyBones m_Bone;
        [SerializeField] private Vector3 m_Euler;
        [SerializeField] private float m_Weight;

        // PROPERTIES: ----------------------------------------------------------------------------

        public HumanBodyBones Bone => this.m_Bone;
        
        public Quaternion Rotation => Quaternion.Euler(this.m_Euler);
        
        public float Weight => this.m_Weight;

        public bool IsValid => this.Transform != null;
        
        [field: NonSerialized] public Transform Transform { get; set; }
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public LookSection(HumanBodyBones bone, float weight)
        {
            this.m_Bone = bone;
            this.m_Euler = Vector3.zero;
            this.m_Weight = weight;
        }

        // public LookSection(HumanBodyBones bone, float weight, Vector3 euler, Transform transform) : this(bone, weight)
        // {
        //     this.m_Euler = euler;
        //     this.Transform = transform;
        // }
    }
}