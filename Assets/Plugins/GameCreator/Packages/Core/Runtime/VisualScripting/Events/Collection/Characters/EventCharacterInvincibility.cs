using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Invincibility Change")]
    [Image(typeof(IconDiamondSolid), ColorTheme.Type.Yellow)]
    
    [Category("Characters/Combat/On Invincibility Change")]
    [Description("Executed when the character's Invincibility changes")]
    
    [Serializable]
    public class EventCharacterInvincibility : TEventCharacter
    {
        private enum Mode
        {
            OnChange,
            OnBecomeInvincible,
            OnBecomeVincible
        }
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private Mode m_Mode = Mode.OnChange;
        
        // PROTECTED METHODS: ---------------------------------------------------------------------
        
        protected override void WhenEnabled(Trigger trigger, Character character)
        {
            character.Combat.Invincibility.EventChange += this.OnInvincibility;
        }

        protected override void WhenDisabled(Trigger trigger, Character character)
        {
            character.Combat.Invincibility.EventChange -= this.OnInvincibility;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnInvincibility(bool isInvincible)
        {
            Character character = this.m_Character.Get<Character>(this.m_Trigger.gameObject);
            if (character == null) return;

            switch (this.m_Mode)
            {
                case Mode.OnChange:
                    _ = this.m_Trigger.Execute(character.gameObject);
                    break;
                
                case Mode.OnBecomeInvincible:
                    if (isInvincible) _ = this.m_Trigger.Execute(character.gameObject);
                    break;
                
                case Mode.OnBecomeVincible:
                    if (!isInvincible) _ = this.m_Trigger.Execute(character.gameObject);
                    break;
                
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}