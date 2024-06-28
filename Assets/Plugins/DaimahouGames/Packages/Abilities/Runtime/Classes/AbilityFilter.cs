using System;
using DaimahouGames.Runtime.Core;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;

namespace DaimahouGames.Runtime.Abilities
{
    [Title("Filter")]
    [Image(typeof(IconFilter), ColorTheme.Type.Gray)]
    
    [Serializable]
    public abstract class AbilityFilter : AbilityStrategy, IEnable
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|
        
        public bool Enabled { get; set; } = true;
        public override string Title => $"Filter [{Summary}]";
        protected abstract string Summary { get; }
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        
        public bool Filter(ExtendedArgs args) => Enabled && Filter_Internal(args);

        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected abstract bool Filter_Internal(ExtendedArgs args);
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}