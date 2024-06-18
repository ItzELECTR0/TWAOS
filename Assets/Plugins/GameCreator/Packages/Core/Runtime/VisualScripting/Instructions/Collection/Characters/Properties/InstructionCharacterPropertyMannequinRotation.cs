using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Mannequin Rotation")]
    [Description("Changes the local rotation of the Mannequin object within the Character")]

    [Category("Characters/Properties/Mannequin Rotation")]

    [Parameter("Character", "The character target")]
    [Parameter("Rotation", "The Local Rotation of the Mannequin")]
    
    [Keywords("Location", "Model", "Local")]
    [Image(typeof(IconBust), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterPropertyMannequinRotation : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] 
        private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        [Space] [SerializeField]
        private PropertyGetRotation m_Rotation = new PropertyGetRotation();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Mannequin Rotation {this.m_Character} = {this.m_Rotation}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;
            
            Quaternion value = this.m_Rotation.Get(args);
            
            character.Animim.Rotation = value;
            character.Animim.ApplyMannequinRotation();
            
            return DefaultResult;
        }
    }
}