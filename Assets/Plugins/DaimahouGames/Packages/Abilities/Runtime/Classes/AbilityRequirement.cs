using System;
using DaimahouGames.Runtime.Core;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;

namespace DaimahouGames.Runtime.Abilities
{
    [Title("Requirement")]
    [Image(typeof(IconFilter), ColorTheme.Type.Gray)]
    
    [Serializable]
    public abstract class AbilityRequirement : AbilityStrategy, IEnable
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|
        
        public bool Enabled { get; set; } = true;
        public override string Title => string.Format("Required {0}[{1}]",
            string.IsNullOrEmpty(RequirementTypeInfo) ? "" : $"<i>for {RequirementTypeInfo}</i> ",
            Summary
        );
        public virtual string RequirementTypeInfo => "Use";
        protected virtual string Summary { get; }
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public bool CheckUsageRequirement(ExtendedArgs args)
        {
            return !Enabled || CheckUsageRequirement_Internal(args);
        }
        
        public bool CheckActivationRequirement(ExtendedArgs args)
        {
            return !Enabled || CheckActivationRequirement_Internal(args);
        }
        
        public virtual void Commit(ExtendedArgs args) {}
        
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        
        protected virtual bool CheckUsageRequirement_Internal(ExtendedArgs args) => true;
        protected virtual bool CheckActivationRequirement_Internal(ExtendedArgs args) => true;
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}