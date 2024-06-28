using System;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;

namespace DaimahouGames.Runtime.Abilities
{
    [Category("Projectile")]
    [Image(typeof(IconProjectile), ColorTheme.Type.Red)]
    
    [Description("Filter projectile collisions")]
    
    [Serializable]
    public class AbilityFilterProjectile : AbilityFilter
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        protected override string Summary => "Projectiles";

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected override bool Filter_Internal(ExtendedArgs args)
        {
            return args.Target != null && args.Target.GetComponent<RuntimeProjectile>() != null;
        }
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}