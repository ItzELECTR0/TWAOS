using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Revive Character")]
    [Description("Changes the state of the Character to alive")]

    [Category("Characters/Properties/Revive Character")]

    [Parameter("Character", "The character target")]

    [Keywords("Respawn", "Alive", "Resurrect")]
    [Image(typeof(IconSkull), ColorTheme.Type.Green)]

    [Serializable]
    public class InstructionCharacterRevive : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField]
        private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Revive {this.m_Character}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            character.IsDead = false;
            return DefaultResult;
        }
    }
}