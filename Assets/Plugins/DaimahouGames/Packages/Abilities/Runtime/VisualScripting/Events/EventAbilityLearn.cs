﻿using System;
using DaimahouGames.Runtime.Core.Common;
using DaimahouGames.Runtime.Core;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;
using Event = GameCreator.Runtime.VisualScripting.Event;

namespace DaimahouGames.Runtime.Abilities.VisualScripting
{
    [Title("On Ability Learned")]
    [Category("Abilities/On Ability Learned")]
    [Description("Executed when an ability is learned.")]

    [Image(typeof(IconAimTarget), ColorTheme.Type.Blue)]

    [Parameter("Caster", "The character learning the ability.")]

    [Keywords("Target")]

    [Serializable]
    public class EventAbilityLearn : Event
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        
        [SerializeField] private PropertyGetGameObject m_Caster = GetGameObjectPlayer.Create();
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        
        [NonSerialized] private IDisposable m_Disposable;
        // todo remove this
        [NonSerialized] private ExtendedArgs m_Args;
        
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        
        protected override void OnStart(Trigger trigger)
        {
            Unsubscribe();
            Subscribe(trigger);
        }

        protected override void OnEnable(Trigger trigger)
        {
            Subscribe(trigger);
        }

        protected override void OnDisable(Trigger trigger)
        {
            Unsubscribe();
        }
        
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|

        private void Subscribe(Component trigger)
        {
            var caster = m_Caster.Get<Caster>(Args.EMPTY);
            if (caster == null) return;

            m_Disposable = caster.OnLearn(OnAbilityLearn);
        }

        private void OnAbilityLearn(Ability ability)
        {
            m_Args = new ExtendedArgs(m_Caster.Get(Args.EMPTY));
            m_Args.Set(ability);
            _ = m_Trigger.Execute(m_Args);
        }
        
        private void Unsubscribe()
        {
            m_Disposable?.Dispose();
        }
        
        //============================================================================================================||
    }
}