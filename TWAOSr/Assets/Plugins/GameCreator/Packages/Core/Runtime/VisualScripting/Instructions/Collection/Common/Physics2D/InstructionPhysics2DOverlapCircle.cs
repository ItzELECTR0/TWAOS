using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Overlap Circle 2D")]
    [Category("Physics 3D/Overlap Circle 3D")]
    
    [Description("Captures all colliders caught inside a Circle defined by a point and radius")]
    [Image(typeof(IconCircleOutline), ColorTheme.Type.Green, typeof(OverlayPhysics))]

    [Parameter(
        "Center", 
        "The center of the circle"
    )]
    
    [Parameter(
        "Radius", 
        "The radius of the circle"
    )]
    
    [Serializable]
    public class InstructionPhysics2DOverlapCircle : TInstructionPhysics2DOverlap
    {
        [SerializeField] 
        private PropertyGetPosition m_Center = GetPositionCharacter.Create;

        [SerializeField] 
        private PropertyGetDecimal m_Radius = GetDecimalDecimal.Create(5f);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Overlap Circle at {this.m_Center}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override int GetColliders(Collider2D[] colliders, Args args)
        {
            Vector3 center = this.m_Center.Get(args);
            float radius = (float) this.m_Radius.Get(args);

            return Physics2D.OverlapCircleNonAlloc(
                center, radius, 
                colliders, this.m_LayerMask
            );
        }
    }
}