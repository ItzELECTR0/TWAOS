using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class HandleField
    {
        public enum Type
        {
            Value,
            Handle
        }
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private Type m_Type = Type.Value;

        [SerializeField] private Bone m_Bone = new Bone(HumanBodyBones.RightHand);
        [SerializeField] private Vector3 m_LocalPosition = Vector3.zero;
        [SerializeField] private Vector3 m_LocalRotation = Vector3.zero;

        [SerializeField] private Handle m_Handle;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public HandleResult Get(Args args)
        {
            return this.m_Type switch
            {
                Type.Value => new HandleResult(
                    this.m_Bone, 
                    this.m_LocalPosition,
                    Quaternion.Euler(this.m_LocalRotation)
                ),
                Type.Handle => this.m_Handle != null 
                    ? this.m_Handle.Get(args)
                    : HandleResult.None,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        // STRING METHOD: -------------------------------------------------------------------------

        public override string ToString()
        {
            return this.m_Type switch
            {
                Type.Value => this.m_Bone.ToString(),
                Type.Handle => this.m_Handle != null ? this.m_Handle.ToString() : "(none)",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}