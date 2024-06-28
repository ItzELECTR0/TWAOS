using System;
using System.Linq;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Version(1, 0, 0)]
    
    [Category("Composite/Conditional Effect")]
    
    [Description("Add a condition to an effect")]

    [Keywords("Conditional")]

    [Image(typeof(IconCondition), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class AbilityEffectCompositeConditional : AbilityEffectComposite
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|

        [SerializeReference] private AbilityRequirement[] m_Requirements;

        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        protected override string Summary => string.Format("{0} {1}",
            $"({m_Requirements.Length}) condition" + (m_Requirements.Length > 1 ? "s" : ""),
            $"({EffectsCount}) effect" + (EffectsCount > 1 ? "s" : "")
        );

        public override string TitleHeader => "[Conditional effects]";

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public override void Apply(ExtendedArgs args)
        {
            if (!Enabled) return;
            throw new NotImplementedException();
            // if (!Enabled || m_Requirements.Any(requirement => !requirement.CheckActivationRequirement(ability)))
            // {
            //     return;
            // }
            //
            // base.Apply(args);
        }
        
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}