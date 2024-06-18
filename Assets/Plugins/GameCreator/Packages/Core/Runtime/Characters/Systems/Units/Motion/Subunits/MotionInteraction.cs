using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class MotionInteraction
    {
        private static readonly Color COLOR_GIZMOS = new Color(0f, 1f, 0f, 0.05f);
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] protected float m_Radius = 2f;
        [SerializeField] protected InteractionMode m_Focus = new InteractionMode();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public float Radius
        {
            get => this.m_Radius;
            set => this.m_Radius = value;
        }
        
        public InteractionMode Mode
        {
            get => this.m_Focus;
            set => this.m_Focus = value;
        }

        // GIZMOS: --------------------------------------------------------------------------------
        
        public void DrawGizmos(Character character)
        {
            this.m_Focus.DrawGizmos(character);
            
            Gizmos.color = COLOR_GIZMOS;
            GizmosExtension.Octahedron(
                character.transform.position,
                Quaternion.identity,
                this.m_Radius
            );
        }
    }
}
