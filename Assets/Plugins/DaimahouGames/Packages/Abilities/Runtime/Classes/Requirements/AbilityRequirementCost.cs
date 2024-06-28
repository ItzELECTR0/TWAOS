using System;
using DaimahouGames.Core.Runtime.Common;
using DaimahouGames.Runtime.Abilities;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Category("Custom Costs")]
    
    [Description("Custom costs which are execute when the ability activate")]
    
    [Image(typeof(IconCondition), ColorTheme.Type.Red)]
    
    [Serializable]
    public class AbilityRequirementCost : AbilityRequirement
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|

        [SerializeField] private string m_Description;
        [SerializeField] private InstructionList m_Cost;

        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public override string RequirementTypeInfo => "costs";
        protected override string Summary => string.Format("{0}", 
            string.IsNullOrEmpty(m_Description) ? "Generic costs" : m_Description
        );

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public override async void Commit(ExtendedArgs args)
        {
            await m_Cost.Run(args);
        }
        
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected override bool CheckUsageRequirement_Internal(ExtendedArgs args)
        {
            return true;
        }

        protected override bool CheckActivationRequirement_Internal(ExtendedArgs args)
        {
            return true;
        }
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}