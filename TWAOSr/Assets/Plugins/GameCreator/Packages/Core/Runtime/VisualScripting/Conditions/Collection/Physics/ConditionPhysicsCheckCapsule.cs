using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Check Capsule")]
    [Description("Returns true if casting a capsule at a position collides with something")]

    [Category("Physics/Check Capsule")]
    
    [Parameter("Position", "The scene position where the capsule's center is cast")]
    [Parameter("Height", "The height of the capsule in Unity units")]
    [Parameter("Radius", "The radius of the capsule in Unity units")]
    [Parameter("Layer Mask", "A bitmask that skips any objects that don't belong to the list")]

    [Example(
        "Note that this Instruction uses Unity's 3D physics engine. " +
        "It won't collide with any 2D objects"
    )]
    
    [Keywords("Check", "Collide", "Touch", "Suit", "Character", "Fit", "3D")]
    
    [Image(typeof(IconCapsuleSolid), ColorTheme.Type.Green)]
    [Serializable]
    public class ConditionPhysicsCheckCapsule : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetPosition m_Position = GetPositionCharactersPlayer.Create;
        [SerializeField] private PropertyGetDecimal m_Height = GetDecimalCharacterHeight.Create;
        [SerializeField] private PropertyGetDecimal m_Radius = GetDecimalCharacterRadius.Create;
        [SerializeField] private LayerMask m_LayerMask = Physics.DefaultRaycastLayers;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"check Capsule at {this.m_Position}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Vector3 position = this.m_Position.Get(args);
            float height = (float) this.m_Height.Get(args);
            
            return Physics.CheckCapsule(
                position + Vector3.up * (height * 0.5f),
                position - Vector3.up * (height * 0.5f),
                (float) this.m_Radius.Get(args),
                this.m_LayerMask,
                QueryTriggerInteraction.Ignore
            );
        }
    }
}
