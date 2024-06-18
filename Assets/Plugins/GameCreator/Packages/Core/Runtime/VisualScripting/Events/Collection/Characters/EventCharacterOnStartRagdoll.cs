using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Start Ragdoll")]
    [Image(typeof(IconSkeleton), ColorTheme.Type.Blue)]
    
    [Category("Characters/Ragdoll/On Start Ragdoll")]
    [Description("Executed when the character enters the ragdoll mode")]

    [Serializable]
    public class EventCharacterOnStartRagdoll : TEventCharacter
    {
        protected override void WhenEnabled(Trigger trigger, Character character)
        {
            character.Ragdoll.EventAfterStartRagdoll += this.OnRagdoll;
        }

        protected override void WhenDisabled(Trigger trigger, Character character)
        {
            character.Ragdoll.EventAfterStartRagdoll -= this.OnRagdoll;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnRagdoll()
        {
            Character character = this.m_Character.Get<Character>(this.m_Trigger.gameObject);
            if (character != null) _ = this.m_Trigger.Execute(character.gameObject);
        }
    }
}