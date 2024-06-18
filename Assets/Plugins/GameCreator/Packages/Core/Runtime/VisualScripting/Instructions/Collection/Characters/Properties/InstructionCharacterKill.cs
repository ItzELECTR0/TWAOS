using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Kill Character")]
    [Description("Changes the state of the Character to dead")]

    [Category("Characters/Properties/Kill Character")]

    [Parameter("Character", "The character target")]

    [Keywords("Dead", "Die", "Murder")]
    [Image(typeof(IconSkull), ColorTheme.Type.Red)]

    [Serializable]
    public class InstructionCharacterKill : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] 
        private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Kill {this.m_Character}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            character.IsDead = true;
            return DefaultResult;
        }
    }
}