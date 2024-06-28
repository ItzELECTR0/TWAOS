using System;
using DaimahouGames.Runtime.Core.Common;
using DaimahouGames.Runtime.Pawns;
using GameCreator.Runtime.Common;

namespace DaimahouGames.Runtime.Abilities
{
    [Category("In development/[Place holder] Faction")]
    [Description("Filter enemies, neutral or ally targets out from the valid target pool.")]
    
    [Image(typeof(IconAimTarget), ColorTheme.Type.Red)]
    
    [Serializable]
    public class AbilityFilterFaction : AbilityFilter
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public override string Title => "[PLACE HOLDER] " + base.Title;
        protected override string Summary => $"Enemies / Friends";
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        protected override bool Filter_Internal(ExtendedArgs args)
        {
            throw new NotImplementedException();
        }
        
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}