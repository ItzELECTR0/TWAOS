using System;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Version(1, 0, 0)]
    
    [Category("Composite/Delayed Effect")]
    
    [Description("Delay an effect")]

    [Keywords("Delay")]

    [Image(typeof(IconTimer), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class AbilityEffectCompositeDelay : AbilityEffectComposite
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|

        [SerializeField] private float m_Delay = 0.25f;
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        protected override string Summary => $"({EffectsCount}) delayed instructions";
        public override string TitleHeader => $"[{m_Delay}s]";

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public override async void Apply(ExtendedArgs args)
        {
            if (!Enabled) return;

            var argsClone = args.Clone;
            await Awaiters.Seconds(m_Delay);
            
            base.Apply(argsClone);
        }
        
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}