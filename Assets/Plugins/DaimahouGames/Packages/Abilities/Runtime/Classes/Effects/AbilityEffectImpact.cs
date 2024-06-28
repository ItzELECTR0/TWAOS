using System;
using System.Collections.Generic;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Version(0, 1, 1)]
    
    [Title("Impact")]
    [Category("Impact")]
    
    [Description("Spawn an impact effect")]

    [Keywords("Explosion", "Impact", "aoe")]

    [Image(typeof(IconExplosion), ColorTheme.Type.Red)]
    [Serializable]
    public class AbilityEffectImpact : AbilityEffect
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|

        [SerializeField] private Impact m_Impact;
        [SerializeField] private List<AbilityEffect> m_Effects;

        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        protected override string Summary => $"{(m_Impact ? m_Impact.name : "none")}";
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        protected override void Apply_Internal(ExtendedArgs args)
        {
            var impact = m_Impact.Get
            (
                args, 
                args.Get<Target>().Position,
                args.Self.transform.rotation
            );
            
            impact.Execute(args);
        }
        
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}