using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Image(typeof(IconCog), ColorTheme.Type.Blue)]
    public abstract class AbilityEffectComposite : AbilityEffect
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        
        [SerializeReference] private AbilityEffect[] m_Effects;
        [SerializeReference] private string m_Descriptor;
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public sealed override string Title => string.Format("{0} {1}",
            TitleHeader,
            string.IsNullOrEmpty(m_Descriptor) ? Summary : m_Descriptor   
        );
        
        public virtual string TitleHeader { get; }
        
        public int EffectsCount => m_Effects.Length;
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public override void Apply(ExtendedArgs args)
        {
            if (!Enabled) return;
            
            foreach (var effect in m_Effects) effect.Apply(args);
        }

        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}