using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Target Change")]
    [Image(typeof(IconBullsEye), ColorTheme.Type.Yellow)]
    
    [Category("Characters/Combat/On Target Change")]
    [Description("Executed every time the character's combat Target changes")]
    
    [Keywords("Focus", "Combat", "Aim")]

    [Serializable]
    public class EventCharacterOnTarget : TEventCharacter
    {
        protected override void WhenEnabled(Trigger trigger, Character character)
        {
            character.Combat.Targets.EventChangeTarget -= this.OnChangeTarget;
            character.Combat.Targets.EventChangeTarget += this.OnChangeTarget;
        }

        protected override void WhenDisabled(Trigger trigger, Character character)
        {
            character.Combat.Targets.EventChangeTarget -= this.OnChangeTarget;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnChangeTarget(GameObject newTarget)
        {
            _ = this.m_Trigger.Execute(newTarget);
        }
    }
}