using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Cycle Previous Target")]
    [Description("Cycles to the previous candidate target from the Targets list")]

    [Category("Characters/Combat/Targeting/Cycle Previous Target")]
    
    [Parameter("Character", "The Character that attempts to change its candidate target")]

    [Keywords("Character", "Combat", "Focus", "Pick", "Candidate", "Targets")]
    [Image(typeof(IconBullsEye), ColorTheme.Type.Yellow, typeof(OverlayArrowLeft))]

    [Serializable]
    public class InstructionCharacterTargetsPrevious : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override string Title => $"Cycle Previous Target from {this.m_Character}";

        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            CycleTargets.Previous(character);
            return DefaultResult;
        }
    }
}