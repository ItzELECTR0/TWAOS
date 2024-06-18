using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Set Player Input")]
    [Description("Changes how the Player Character reacts to input commands")]

    [Category("Characters/Player/Set Player Input")]
    
    [Parameter("Character", "The Character that changes its Player Input behavior")]
    [Parameter("Input", "The new input method that the Character starts to listen")]

    [Keywords("Character", "Button", "Control", "Keyboard", "Mouse", "Gamepad", "Joystick")]
    [Image(typeof(IconGamepadCross), ColorTheme.Type.Green)]

    [Serializable]
    public class InstructionCharacterPlayer : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        [SerializeField] private UnitPlayer m_Input = new UnitPlayer();

        public override string Title => 
            $"Change Player Input on {this.m_Character} to {this.m_Input}";

        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            character.Kernel.ChangePlayer(character, this.m_Input.Wrapper);
            return DefaultResult;
        }
    }
}