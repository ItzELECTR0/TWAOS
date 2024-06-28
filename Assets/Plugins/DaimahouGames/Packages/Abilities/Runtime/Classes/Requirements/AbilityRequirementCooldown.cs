using System;
using DaimahouGames.Runtime.Core;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Category("Cooldown Requirement")]
    [Image(typeof(IconClock), ColorTheme.Type.Teal)]
    
    [Description("Ability is only usable when not in cooldown. Ability will enter cooldown" +
                 " automatically and fail to start for the entire cooldown duration.")]
    
    [Serializable]
    public class AbilityRequirementCooldown : AbilityRequirement
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|

        [SerializeField] private PropertyGetDecimal m_Cooldown;
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|
        
        public override string Title => string.Format(
            "Cooldown [{0}s]", 
            m_Cooldown
        );

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected override bool CheckUsageRequirement_Internal(ExtendedArgs args)
        {
            var ability = args.Get<RuntimeAbility>();
            return !ability.Caster.Get<Cooldowns>().IsInCooldown(ability.ID);
        }

        public override void Commit(ExtendedArgs args)
        {
            var ability = args.Get<RuntimeAbility>();
            ability.Caster.Get<Cooldowns>().AddCooldown(ability.ID, (float) m_Cooldown.Get(args));
        }
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}