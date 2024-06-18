using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Clear Target")]
    [Description("Clears the targeted game object by the specified Character")]

    [Category("Characters/Combat/Targeting/Clear Target")]
    
    [Parameter("Character", "The Character that attempts to change its target")]

    [Keywords("Character", "Combat", "Focus", "Pick")]
    [Image(typeof(IconBullsEye), ColorTheme.Type.Red)]

    [Serializable]
    public class InstructionCharacterClearTarget : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override string Title => $"Target {this.m_Character} = none";

        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;
            
            character.Combat.Targets.Primary = null;
            return DefaultResult;
        }
    }
}