using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Image(typeof(IconTarget), ColorTheme.Type.Gray)]
    
    [Serializable]
    public abstract class AbilityTargeting : AbilityStrategy
    {
        //============================================================================================================||
        // ------------------------------------------------------------------------------------------------------------|
        
        protected const string INPUT_SUCCESS = "Input processed";

        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        
        public abstract void AcquireTargets(ExtendedArgs args);
        
        public abstract Task ProcessInput(ExtendedArgs args);

        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        
        public void RegisterTargets(ExtendedArgs args)
        {
            var ability = args.Get<RuntimeAbility>();
            if (args.Has<Target>())
            {
                if(!ability.Filter(args)) ability.Targets.Add(args.Get<Target>());
            }
            else
            {
                foreach (var target in args.Get<List<Target>>())
                {
                    args.Set(target);
                    if(!ability.Filter(args)) ability.Targets.Add(target);
                }
            }
        }

        public void ValidateTargets(ExtendedArgs args)
        {
            if (args.Has<Target>() || args.Has<List<Target>>()) return;

            var ability = args.Get<RuntimeAbility>();
            ability.Cancel();
            ability.OnStatus.Send($"No target found");
        }
        
        //============================================================================================================||
    }
}