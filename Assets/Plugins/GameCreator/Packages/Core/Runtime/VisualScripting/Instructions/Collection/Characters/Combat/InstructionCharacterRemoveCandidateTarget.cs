using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Remove Target Candidate")]
    [Description("Removes a new candidate target for the specified character")]

    [Category("Characters/Combat/Targeting/Remove Target Candidate")]
    
    [Parameter("Character", "The Character that attempts to change its target candidate")]
    [Parameter("Target", "The target candidate to remove by the character")]

    [Keywords("Character", "Combat", "Focus", "Pick")]
    [Image(typeof(IconBullsEye), ColorTheme.Type.TextLight, typeof(OverlayMinus))]

    [Serializable]
    public class InstructionCharacterRemoveCandidateTarget : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        [SerializeField] private PropertyGetGameObject m_Target = GetGameObjectInstance.Create();

        public override string Title => $"Remove {this.m_Target} Candidate from {this.m_Character}";

        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            GameObject target = this.m_Target.Get(args);
            character.Combat.Targets.RemoveCandidate(target);
            
            return DefaultResult;
        }
    }
}