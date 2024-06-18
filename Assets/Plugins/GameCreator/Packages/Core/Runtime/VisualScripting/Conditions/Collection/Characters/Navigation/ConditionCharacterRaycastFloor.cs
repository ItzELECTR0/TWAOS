using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Raycast Floor")]
    [Description("Returns true if there is an obstacle the specified units below the character")]

    [Category("Characters/Navigation/Raycast Floor")]

    [Keywords("Floor", "Stand", "Land", "Ground", "Obstacle")]
    
    [Image(typeof(IconCharacterWalk), ColorTheme.Type.Blue, typeof(OverlayArrowDown))]
    [Serializable]
    public class ConditionCharacterRaycastFloor : TConditionCharacter
    {
        [SerializeField] private LayerMask m_LayerMask = Physics.DefaultRaycastLayers;
        [SerializeField] private PropertyGetDecimal m_Distance = GetDecimalDecimal.Create(5f);
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"is floor below {this.m_Character} {this.m_Distance}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            
            if (character == null) return false;
            if (character.Driver.IsGrounded) return true;

            float distance = (float) this.m_Distance.Get(args);
            return Physics.Raycast(
                character.Feet,
                Vector3.down, 
                distance,
                this.m_LayerMask,
                QueryTriggerInteraction.Ignore
            );
        }
    }
}
