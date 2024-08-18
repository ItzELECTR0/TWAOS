using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Check Character 3D Fits")]
    [Description("Returns true if the character fits with the new radius and height values")]

    [Category("Physics/Check Character 3D Fits")]
    
    [Parameter("Character", "The character to check")]
    [Parameter("Height", "The height of the character in Unity units")]
    [Parameter("Radius", "The radius of the character in Unity units")]
    [Parameter("Layer Mask", "A bitmask that skips any objects that don't belong to the list")]

    [Example(
        "Note that this Instruction uses Unity's 3D physics engine. " +
        "It won't collide with any 2D objects"
    )]
    
    [Keywords("Check", "Collide", "Capsule", "Touch", "Suit", "Character", "Fit", "3D")]
    
    [Image(typeof(IconCharacter), ColorTheme.Type.Green, typeof(OverlayPhysics))]
    [Serializable]
    public class ConditionPhysicsCharacter3DFits : Condition
    {
        private const float SAFE_OFFSET = 0.005f;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        [SerializeField] private PropertyGetDecimal m_Height = GetDecimalCharacterHeight.Create;
        [SerializeField] private PropertyGetDecimal m_Radius = GetDecimalCharacterRadius.Create;
        [SerializeField] private LayerMask m_LayerMask = Physics.DefaultRaycastLayers;

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private Collider[] m_Hits = new Collider[32];
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"check {this.m_Character} fits";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return false;
            
            Vector3 bottom = character.transform.position - Vector3.up * (character.Motion.Height * 0.5f);
            float height = (float) this.m_Height.Get(args);
            float radius = (float) this.m_Radius.Get(args);
            
            int hits = Physics.OverlapCapsuleNonAlloc(
                bottom + Vector3.up * (radius + SAFE_OFFSET),
                bottom + Vector3.up * (radius + height + radius),
                radius - SAFE_OFFSET,
                this.m_Hits,
                this.m_LayerMask,
                QueryTriggerInteraction.Ignore
            );
            
            for (int i = 0; i < hits; ++i)
            {
                Collider hit = this.m_Hits[i];
                
                if (hit == null) continue;
                if (hit.transform.IsChildOf(character.transform)) continue;
                
                return false;
            }

            return true;
        }
    }
}
