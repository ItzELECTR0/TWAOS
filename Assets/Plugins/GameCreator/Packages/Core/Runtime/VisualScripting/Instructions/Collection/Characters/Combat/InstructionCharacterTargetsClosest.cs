using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Cycle Closest Target")]
    [Description("Cycles to the closest candidate target to the character from the Targets list")]

    [Category("Characters/Combat/Targeting/Cycle Closest Target")]
    
    [Parameter("Character", "The Character that attempts to change its candidate target")]

    [Keywords("Character", "Combat", "Focus", "Pick", "Candidate", "Targets")]
    [Image(typeof(IconBullsEye), ColorTheme.Type.Yellow, typeof(OverlayDot))]

    [Serializable]
    public class InstructionCharacterTargetsClosest : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override string Title => $"Cycle Closest Target from {this.m_Character}";

        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            CycleTargets.Closest(character);
            return DefaultResult;
        }
    }
}