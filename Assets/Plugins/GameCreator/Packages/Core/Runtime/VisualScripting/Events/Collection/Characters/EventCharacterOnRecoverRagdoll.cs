using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Recover Ragdoll")]
    [Image(typeof(IconSkeleton), ColorTheme.Type.Green)]
    
    [Category("Characters/Ragdoll/On Recover Ragdoll")]
    [Description("Executed when the character recovers from the ragdoll mode")]

    [Serializable]
    public class EventCharacterOnRecoverRagdoll : TEventCharacter
    {
        protected override void WhenEnabled(Trigger trigger, Character character)
        {
            character.Ragdoll.EventAfterStartRecover += this.OnRagdoll;
        }

        protected override void WhenDisabled(Trigger trigger, Character character)
        {
            character.Ragdoll.EventAfterStartRecover -= this.OnRagdoll;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnRagdoll()
        {
            Character character = this.m_Character.Get<Character>(this.m_Trigger.gameObject);
            if (character != null) _ = this.m_Trigger.Execute(character.gameObject);
        }
    }
}