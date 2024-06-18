using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Step")]
    [Image(typeof(IconFootprint), ColorTheme.Type.Yellow)]
    
    [Category("Characters/Navigation/On Step")]
    [Description("Executed every time the character takes a step")]
    
    [Keywords("Footstep", "Foot", "Feet", "Ground")]
    
    // TODO: [10/3/2023] Remove in a year
    [MovedFrom(false, null, null, "EventOnCharacterStep")]

    [Serializable]
    public class EventCharacterOnStep : TEventCharacter
    {
        protected override void WhenEnabled(Trigger trigger, Character character)
        {
            character.Footsteps.EventStep += this.OnStep;
        }

        protected override void WhenDisabled(Trigger trigger, Character character)
        {
            character.Footsteps.EventStep -= this.OnStep;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnStep(Transform foot)
        {
            _ = this.m_Trigger.Execute(foot.gameObject);
        }
    }
}