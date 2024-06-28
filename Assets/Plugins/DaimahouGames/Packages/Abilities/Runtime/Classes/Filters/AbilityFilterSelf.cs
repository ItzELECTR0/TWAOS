using System;
using DaimahouGames.Runtime.Core.Common;
using DaimahouGames.Runtime.Pawns;
using GameCreator.Runtime.Common;

namespace DaimahouGames.Runtime.Abilities
{
    [Category("Self")]
    [Image(typeof(IconSelf), ColorTheme.Type.Red)]
    
    [Description("Remove the caster from valid targets.")]
    
    [Serializable]
    public class AbilityFilterSelf : AbilityFilter
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        protected override string Summary => "Self";
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        protected override bool Filter_Internal(ExtendedArgs args)
        {
            return args.Get<RuntimeAbility>()?.Caster?.GameObject == args.Get<Target>().GameObject;
        }
        
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}