using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Add Target Candidate")]
    [Description("Adds a new candidate target for the specified character")]

    [Category("Characters/Combat/Targeting/Add Target Candidate")]
    
    [Parameter("Character", "The Character that attempts to change its candidate target")]
    [Parameter("Target", "The new target candidate game object by the character")]

    [Keywords("Character", "Combat", "Focus", "Pick")]
    [Image(typeof(IconBullsEye), ColorTheme.Type.TextLight, typeof(OverlayPlus))]

    [Serializable]
    public class InstructionCharacterAddCandidateTarget : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        [SerializeField] private PropertyGetGameObject m_Target = GetGameObjectInstance.Create();

        public override string Title => $"Add {this.m_Target} Candidate to {this.m_Character}";

        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            GameObject target = this.m_Target.Get(args);
            character.Combat.Targets.AddCandidate(target);
            
            return DefaultResult;
        }
    }
}