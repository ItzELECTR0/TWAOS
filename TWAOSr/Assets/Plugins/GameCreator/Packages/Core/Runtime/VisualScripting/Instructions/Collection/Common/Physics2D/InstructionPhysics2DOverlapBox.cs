using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Overlap Box 2D")]
    [Category("Physics 3D/Overlap Box 2D")]
    
    [Description("Captures all colliders caught inside a box")]
    [Image(typeof(IconSquareOutline), ColorTheme.Type.Green, typeof(OverlayPhysics))]

    [Parameter(
        "Center", 
        "The center of the box"
    )]
    
    [Parameter(
        "Size", 
        "The size of the box in each axis"
    )]
    
    [Parameter(
        "Angle", 
        "The rotation of the box in world space"
    )]
    
    [Keywords("Cube")]
    [Serializable]
    public class InstructionPhysics2DOverlapBox : TInstructionPhysics2DOverlap
    {
        private static readonly RaycastHit2D[] HITS = new RaycastHit2D[LENGTH];
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private PropertyGetPosition m_Center = GetPositionCharacter.Create;

        [SerializeField] 
        private PropertyGetDirection m_Size = GetDirectionVector3Zero.Create();

        [SerializeField]
        private PropertyGetDecimal m_Angle = GetDecimalDecimal.Create(0f);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Overlap Box at {this.m_Center}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override int GetColliders(Collider2D[] colliders, Args args)
        {
            Vector3 center = this.m_Center.Get(args);
            Vector3 size = this.m_Size.Get(args);
            float angle = (float) this.m_Angle.Get(args);

            int hits = Physics2D.BoxCastNonAlloc(
                center, size, angle, Vector2.up,
                HITS, angle,
                this.m_LayerMask
            );

            for (int i = 0; i < hits; ++i)
            {
                colliders[i] = HITS[i].collider;
            }

            return hits;
        }
    }
}