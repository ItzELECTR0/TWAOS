using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Check Sphere")]
    [Description("Returns true if casting a sphere at a position collides with something")]

    [Category("Physics/Check Sphere")]
    
    [Parameter("Position", "The scene position where the sphere's center is cast")]
    [Parameter("Radius", "The radius of the sphere in Unity units")]
    [Parameter("Layer Mask", "A bitmask that skips any objects that don't belong to the list")]

    [Example(
        "Note that this Instruction uses Unity's 3D physics engine. " +
        "It won't collide with any 2D objects"
    )]
    
    [Keywords("Check", "Collide", "Touch", "Suit", "Circle", "Circumference", "Round", "3D")]
    
    [Image(typeof(IconSphereSolid), ColorTheme.Type.Green)]
    [Serializable]
    public class ConditionPhysicsCheckSphere : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetPosition m_Position = GetPositionCharacter.Create;
        [SerializeField] private PropertyGetDecimal m_Radius = new PropertyGetDecimal(0.5f);
        [SerializeField] private LayerMask m_LayerMask = Physics.DefaultRaycastLayers;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"check Sphere at {this.m_Position}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            return Physics.CheckSphere(
                this.m_Position.Get(args),
                (float) this.m_Radius.Get(args),
                this.m_LayerMask,
                QueryTriggerInteraction.Ignore
            );
        }
    }
}
