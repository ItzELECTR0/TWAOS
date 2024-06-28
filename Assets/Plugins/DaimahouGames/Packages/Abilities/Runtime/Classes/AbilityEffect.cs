using System;
using DaimahouGames.Runtime.Core;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Title("Effect")]
    [Image(typeof(IconFilter), ColorTheme.Type.Gray)]
    
    [Serializable]
    public abstract class AbilityEffect : AbilityStrategy, IEnable
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        
        [SerializeField] private bool m_Enabled = true;
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public bool Enabled { get => m_Enabled; set => m_Enabled = value; }
        public override string Title => string.Format(Summary);
        protected virtual string Summary { get; }
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public virtual void Apply(ExtendedArgs args)
        {
            if (!Enabled) return;
            Apply_Internal(args);
        }

        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected virtual void Apply_Internal(ExtendedArgs args) {}
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}