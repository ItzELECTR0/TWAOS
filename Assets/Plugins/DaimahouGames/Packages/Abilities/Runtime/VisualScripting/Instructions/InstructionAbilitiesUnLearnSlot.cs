using System;
using System.Threading.Tasks;
using DaimahouGames.Runtime.Core.Common;
using DaimahouGames.Runtime.Core;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities.VisualScripting
{
    [Version(1, 0, 0)]
    
    [Title("UnLearn Ability")]
    [Category("Abilities/UnLearn from slot")]
    
    [Image(typeof(IconAbility), ColorTheme.Type.Blue)]
    [Description("UnLearn Ability from slot")]

    [Parameter("Target", "The caster game object with a Pawn/Character component")]
    [Parameter("Ability", "The ability to be unlearned")]
    [Parameter("Slot", "The slot to learn the ability in")]

    [Serializable]
    public class InstructionAbilitiesUnLearnSlot : Instruction
    {
        [SerializeField]
        private PropertyGetGameObject m_Caster = GetGameObjectPlayer.Create();
        
        [SerializeField] private PropertyGetDecimal m_Slot = new PropertyGetInteger();
        
        public override string Title => string.Format(
            "{0} UnLearns ability from slot[{1}]",
            m_Caster,
            m_Slot 
        );

        protected override Task Run(Args args)
        {
            var caster = m_Caster.Get<Caster>(args);
            var slot = Mathf.FloorToInt((float) this.m_Slot.Get(args));
            caster.UnLearn(slot);
            return Task.CompletedTask;
        }
    }
}