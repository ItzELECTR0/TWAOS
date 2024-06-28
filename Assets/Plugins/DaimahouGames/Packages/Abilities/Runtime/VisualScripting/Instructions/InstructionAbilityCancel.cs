using System;
using System.Threading.Tasks;
using DaimahouGames.Runtime.Core;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities.VisualScripting
{
    [Version(1, 0, 0)]
    
    [Title("Cancel Ability")]
    [Category("Abilities/Cancel")]
    
    [Image(typeof(IconAbility), ColorTheme.Type.Blue, typeof(OverlayCross))]
    [Description("Instruct the Character to cast an ability")]

    [Serializable]
    public class InstructionAbilityCancel : Instruction
    {
        [SerializeField]
        private PropertyGetGameObject m_Caster = GetGameObjectPlayer.Create();

        public override string Title => string.Format(
            "{0} Cancel Cast",
            m_Caster
        );

        protected override Task Run(Args args)
        {
            var caster = m_Caster.Get<Caster>(args).RequiredOn(m_Caster.Get(args));
            caster.Pawn.GetState<CastState>().Cancel();
            
            return Task.CompletedTask;
        }
    }
}