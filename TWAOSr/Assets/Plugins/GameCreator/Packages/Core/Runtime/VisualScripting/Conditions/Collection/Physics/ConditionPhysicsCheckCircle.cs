using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Check Circle")]
    [Description("Returns true if casting a circle at a position doesn't collide with anything")]

    [Category("Physics/Check Circle")]
    
    [Parameter("Position", "The scene position where the circle's center is cast. Z axis is ignored")]
    [Parameter("Radius", "The radius of the circle in Unity units")]
    [Parameter("Layer Mask", "A bitmask that skips any objects that don't belong to the list")]
    
    [Example(
        "Note that this Instruction uses Unity's 2D physics engine. " +
        "It won't collide with any 3D objects"
    )]

    [Keywords("Check", "Collide", "Touch", "Suit", "Sphere", "Circumference", "Round", "2D")]
    
    [Image(typeof(IconCircleOutline), ColorTheme.Type.Green)]
    [Serializable]
    public class ConditionPhysicsCheckCircle : Condition
    {
        private static readonly Collider2D[] Colliders = new Collider2D[1];
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetPosition m_Position = GetPositionCharacter.Create;
        [SerializeField] private PropertyGetDecimal m_Radius = new PropertyGetDecimal(0.5f);
        [SerializeField] private LayerMask m_LayerMask = Physics.DefaultRaycastLayers;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"check Circle at {this.m_Position}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            int collisionCount = Physics2D.OverlapCircleNonAlloc(
                this.m_Position.Get(args).XY(),
                (float) this.m_Radius.Get(args),
                Colliders,
                this.m_LayerMask
            );
            
            return collisionCount >= 1;
        }
    }
}
