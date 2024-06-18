using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Focus")]
    [Image(typeof(IconCharacterInteract), ColorTheme.Type.Green, typeof(OverlayDot))]
    
    [Category("Interactive/On Focus")]
    [Description("Executed when the Character focuses on this Interactive object")]

    [Serializable]
    public class EventCharacterOnFocusInteractive : TEventCharacter
    {
        // OVERRIDE METHODS: ----------------------------------------------------------------------

        protected internal override void OnAwake(Trigger trigger)
        {
            base.OnAwake(trigger);
            trigger.RequireInteractionTracker();
        }

        protected override void WhenEnabled(Trigger trigger, Character character)
        {
            character.Interaction.EventFocus += this.OnFocus;
        }

        protected override void WhenDisabled(Trigger trigger, Character character)
        {
            character.Interaction.EventFocus -= this.OnFocus;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void OnFocus(Character character, IInteractive interactive)
        {
            if (!this.IsActive) return;
            if (character == null) return;
            if (character.Interaction.Target == null) return;
            
            if (character.Interaction.Target.Instance != this.m_Trigger.gameObject) return;
            _ = this.m_Trigger.Execute(character.gameObject);
        }
    }
}