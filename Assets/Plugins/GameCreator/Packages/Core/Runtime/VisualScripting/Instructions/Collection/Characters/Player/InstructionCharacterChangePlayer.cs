using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Player")]
    [Description("Changes the Character identified as the Player")]

    [Category("Characters/Player/Change Player")]
    
    [Parameter("Character", "The Character becomes the new Player character")]

    [Keywords("Character", "Is", "Control")]
    [Image(typeof(IconPlayer), ColorTheme.Type.Green)]

    [Serializable]
    public class InstructionCharacterChangePlayer : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_NewPlayer = GetGameObjectPlayer.Create();

        public override string Title => $"Change Player to {this.m_NewPlayer}";

        protected override Task Run(Args args)
        {
            Character character = this.m_NewPlayer.Get<Character>(args);
            if (character == null) return DefaultResult;

            Character previous = ShortcutPlayer.Get<Character>();
            if (previous != null) previous.IsPlayer = false;
            
            character.IsPlayer = true;
            return DefaultResult;
        }
    }
}