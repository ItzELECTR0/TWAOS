using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Blur")]
    [Image(typeof(IconCharacterInteract), ColorTheme.Type.Red, typeof(OverlayDot))]
    
    [Category("Interactive/On Blur")]
    [Description("Executed when the Character loses focus on this Interactive object")]

    [Serializable]
    public class EventCharacterOnBlurInteractive : TEventCharacter
    {
        // OVERRIDE METHODS: ----------------------------------------------------------------------

        protected internal override void OnAwake(Trigger trigger)
        {
            base.OnAwake(trigger);
            trigger.RequireInteractionTracker();
        }

        protected override void WhenEnabled(Trigger trigger, Character character)
        {
            character.Interaction.EventBlur += this.OnBlur;
        }

        protected override void WhenDisabled(Trigger trigger, Character character)
        {
            character.Interaction.EventBlur -= this.OnBlur;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void OnBlur(Character character, IInteractive interactive)
        {
            if (!this.IsActive) return;
            if (character == null) return;
            if (character.Interaction.Target == null) return;

            int targetId = character.Interaction.Target.InstanceID;
            int triggerId = this.m_Trigger.gameObject.GetInstanceID(); 
            if (targetId != triggerId) return;
            
            _ = this.m_Trigger.Execute(character.gameObject);
        }
    }
}