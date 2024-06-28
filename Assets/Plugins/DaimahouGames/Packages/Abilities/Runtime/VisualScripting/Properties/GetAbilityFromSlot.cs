using System;
using DaimahouGames.Runtime.Abilities;
using DaimahouGames.Runtime.Core;
using DaimahouGames.Runtime.Pawns;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace Plugins.DaimahouGames.Packages.Abilities.Runtime.VisualScripting.Properties
{
    [Title("From Slot")]
    [Category("From Slot")]
    
    [Image(typeof(IconAbility), ColorTheme.Type.Yellow)]
    [Description("Reference to an ability from Slot")]

    [Serializable] [HideLabelsInEditor]
    public class GetAbilityFromSlot : PropertyTypeGetAbility
    {
        [SerializeField] private PropertyGetGameObject m_Pawn;
        [SerializeField] private PropertyGetDecimal m_Slot;
        
        public override Ability Get(Args args)
        {
            Pawn pawn = this.m_Pawn.Get<Pawn>(args);

            if (pawn == null) return null;
            
            Caster caster = pawn.Get<Caster>();

            return caster?.GetSlottedAbility((int)this.m_Slot.Get(args));
        }

        public static PropertyGetAbility Create => new(
            new GetAbilityFromSlot()
        );

        public override string String => this.m_Pawn != null
            ? $"[{this.m_Pawn}]'s ability slot [{this.m_Slot}]"
            : "(none)";
    }
}