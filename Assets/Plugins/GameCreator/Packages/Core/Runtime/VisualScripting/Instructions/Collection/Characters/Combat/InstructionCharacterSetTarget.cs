using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Set Target")]
    [Description("Changes the targeted game object by the specified Character")]

    [Category("Characters/Combat/Targeting/Set Target")]
    
    [Parameter("Character", "The Character that attempts to change its target")]
    [Parameter("Target", "The new targeted game object by the character")]

    [Keywords("Character", "Combat", "Focus", "Pick")]
    [Image(typeof(IconBullsEye), ColorTheme.Type.Green)]

    [Serializable]
    public class InstructionCharacterSetTarget : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        [SerializeField] private PropertyGetGameObject m_Target = GetGameObjectInstance.Create();

        public override string Title => $"Target {this.m_Character} = {this.m_Target}";

        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            GameObject target = this.m_Target.Get(args);
            character.Combat.Targets.Primary = target;
            
            return DefaultResult;
        }
    }
}