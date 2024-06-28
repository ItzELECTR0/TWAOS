using System;
using System.Linq;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Category("In Range Requirement")]
    [Image(typeof(IconTargetArea), ColorTheme.Type.Teal)]
    
    [Description("After selecting the target location, ability will only activate if it is within range.")]
    
    [Serializable]
    public class AbilityRequirementInRange : AbilityRequirement
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        protected override string Summary => "Target in Range";
        public override string RequirementTypeInfo => "Activation";

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected override bool CheckActivationRequirement_Internal(ExtendedArgs args)
        {
            var ability = args.Get<RuntimeAbility>();
            return ability.Targets.Any(t =>
            {
                var distance = VectorHelper.Distance2D(ability.Caster.Position, t.Position);
                return distance <= ability.GetRange(args);
            });
        }

        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}