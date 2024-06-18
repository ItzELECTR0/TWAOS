using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Serializable]
    public abstract class TEventCharacter : Event
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        // METHODS: -------------------------------------------------------------------------------
        
        protected internal override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);
            
            Character character = this.m_Character.Get<Character>(trigger.gameObject);
            if (character != null) this.WhenEnabled(trigger, character);
        }

        protected internal override void OnStart(Trigger trigger)
        {
            base.OnStart(trigger);
            
            Character character = this.m_Character.Get<Character>(trigger.gameObject);
            if (character == null) return;
            
            this.WhenDisabled(trigger, character);
            this.WhenEnabled(trigger, character);
        }

        protected internal override void OnDisable(Trigger trigger)
        {
            base.OnDisable(trigger);
            
            Character character = this.m_Character.Get<Character>(trigger.gameObject);
            if (character != null) this.WhenDisabled(trigger, character);
        }
        
        // ABSTRACT METHODS: ----------------------------------------------------------------------

        protected abstract void WhenEnabled(Trigger trigger, Character character);
        protected abstract void WhenDisabled(Trigger trigger, Character character);
    }
}