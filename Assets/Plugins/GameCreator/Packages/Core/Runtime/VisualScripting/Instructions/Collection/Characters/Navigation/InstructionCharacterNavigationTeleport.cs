using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Teleport")]
    [Description("Instantaneously moves a Character from its current position to a new one")]

    [Category("Characters/Navigation/Teleport")]
    
    [Parameter("Location", "The position and/or rotation where the Character is teleported")]

    [Keywords("Change", "Position", "Location", "Respawn", "Spawn")]
    [Image(typeof(IconCharacter), ColorTheme.Type.Blue)]

    [Serializable]
    public class InstructionCharacterNavigationTeleport : TInstructionCharacterNavigation
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetLocation m_Location = GetLocationNavigationMarker.Create;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Teleport {this.m_Character} to {this.m_Location}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            Location location = this.m_Location.Get(args);
            
            Vector3 position = location.GetPosition(character.gameObject);
            Quaternion rotation = location.GetRotation(character.gameObject);

            if (location.HasPosition(character.gameObject)) character.Driver.SetPosition(position);
            if (location.HasRotation(character.gameObject)) character.Driver.SetRotation(rotation);
            
            return DefaultResult;
        }
    }
}