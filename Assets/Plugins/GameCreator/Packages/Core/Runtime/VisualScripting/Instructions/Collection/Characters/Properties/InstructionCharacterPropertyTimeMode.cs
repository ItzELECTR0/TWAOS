using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Time Mode")]
    [Description("Changes the Character's Time Mode")]

    [Category("Characters/Properties/Change Time Mode")]
    
    [Parameter("Time Mode", "The target Time Mode for the Character")]

    [Keywords("Scale", "Game")]
    [Image(typeof(IconTimer), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterPropertyTimeMode : TInstructionCharacterProperty
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private TimeMode.UpdateMode m_TimeMode = TimeMode.UpdateMode.GameTime;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Time Mode {this.m_Character} = {this.m_TimeMode}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            character.Time = new TimeMode(this.m_TimeMode);
            return DefaultResult;
        }
    }
}