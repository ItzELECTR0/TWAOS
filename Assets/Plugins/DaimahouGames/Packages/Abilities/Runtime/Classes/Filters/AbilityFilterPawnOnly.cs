using System;
using DaimahouGames.Runtime.Pawns;
using DaimahouGames.Runtime.Core.Common;
using DaimahouGames.Runtime.Core;
using GameCreator.Runtime.Common;

namespace DaimahouGames.Runtime.Abilities
{
    [Category("Pawn only")]
    [Image(typeof(IconPawn), ColorTheme.Type.Red)]
    
    [Description("Only affect pawns (pure location gets removed)")]
    
    [Serializable]
    public class AbilityFilterPawnOnly : AbilityFilter
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        protected override string Summary => "Pawns only";

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected override bool Filter_Internal(ExtendedArgs args)
        {
            return args.Target == null || args.Target.GetComponent<Pawn>() == null;
        }
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}