using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Mannequin Position")]
    [Description("Changes the local position of the Mannequin object within the Character")]

    [Category("Characters/Properties/Mannequin Position")]

    [Parameter("Character", "The character target")]
    [Parameter("Position", "The Local Position of the Mannequin")]
    
    [Keywords("Location", "Model", "Local", "Change", "Set", "Root")]
    [Image(typeof(IconBust), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterPropertyMannequinPosition : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] 
        private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        [Space] [SerializeField]
        private PropertyGetPosition m_Position = new PropertyGetPosition();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Mannequin Position {this.m_Character} = {this.m_Position}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;
            
            Vector3 value = this.m_Position.Get(args);
            
            character.Animim.Position = value;
            character.Animim.ApplyMannequinPosition();
            
            return DefaultResult;
        }
    }
}