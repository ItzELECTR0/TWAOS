using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Interact")]
    [Description("Changes how the Player Character reacts to input commands")]

    [Category("Characters/Interaction/Interact")]
    
    [Parameter("Character", "The Character that attempts to interact")]

    [Keywords("Character", "Button", "Pick", "Do", "Use", "Pull", "Press", "Push", "Talk")]
    [Image(typeof(IconCharacterInteract), ColorTheme.Type.Green)]

    [Serializable]
    public class InstructionCharacterInteract : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override string Title => $"{this.m_Character} do Interact";

        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            character.Interaction.Interact();
            return DefaultResult;
        }
    }
}