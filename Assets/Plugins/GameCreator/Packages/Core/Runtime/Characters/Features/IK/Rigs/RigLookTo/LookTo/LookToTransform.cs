using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters.IK
{
    public readonly struct LookToTransform : ILookTo
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private readonly Transform m_Transform;
        [NonSerialized] private readonly Character m_Character;
        [NonSerialized] private readonly Vector3 m_Offset;

        // PROPERTIES: ----------------------------------------------------------------------------

        [field: NonSerialized] public int Layer { get; }

        public bool Exists => this.m_Transform != null;

        public Vector3 Position
        {
            get
            {
                Vector3 offset = this.m_Transform.TransformDirection(this.m_Offset);
                return this.m_Character != null
                    ? this.m_Character.Eyes + offset
                    : this.m_Transform.position + offset;
            }
        }

        public GameObject Target => this.m_Transform != null ? this.m_Transform.gameObject : null;

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public LookToTransform(int layer, Transform transform, Vector3 offset)
        {
            this.Layer = layer;
            this.m_Transform = transform;
            this.m_Character = transform != null ? transform.Get<Character>() : null;
            this.m_Offset = offset;
        }
    }
}