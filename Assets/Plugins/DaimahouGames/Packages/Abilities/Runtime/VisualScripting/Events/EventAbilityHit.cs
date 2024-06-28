using System;
using DaimahouGames.Runtime.Core.Common;
using DaimahouGames.Runtime.Core;
using DaimahouGames.Runtime.Pawns;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Event = GameCreator.Runtime.VisualScripting.Event;

namespace DaimahouGames.Runtime.Abilities.VisualScripting
{
    [Title("On Ability Hit")]
    [Category("Abilities/On Ability Hit")]
    [Description("Executed when an impact or projectile hits a target.")]

    [Image(typeof(IconAimTarget), ColorTheme.Type.Blue)]

    [Parameter("Pawn", "The character casting the ability.")]

    [Keywords("Target")]

    [Serializable]
    public class EventAbilityHit : Event
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        
        [SerializeField] private PropertyGetGameObject m_Pawn = GetGameObjectPlayer.Create();
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        
        [NonSerialized] private IDisposable m_Disposable;
        
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
            var caster = m_Pawn.Get<Pawn>(trigger.gameObject);
            if (caster == null) return;

            m_Disposable = caster.Message.Subscribe<MessageAbilityHit>(a =>
            {
                _ = m_Trigger.Execute(new ExtendedArgs(caster.gameObject, a.CasterGameObject));
            });
        }
        
        private void Unsubscribe()
        {
            m_Disposable?.Dispose();
        }
        
        //============================================================================================================||
    }
}