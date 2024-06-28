using System;
using System.Threading.Tasks;
using DaimahouGames.Runtime.Characters;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Play Gesture")]
    [Description("Plays a Reactive Gesture on a Character once")]

    [Category("Characters/Animation/Play Reactive Gesture")]

    [Parameter("Character", "The character target")]
    [Parameter("ReactiveGesture", "The Reactive gesture that is played")]
    [Image(typeof(IconCharacterGesture), ColorTheme.Type.Blue)]

    [Keywords("Reactive Gesture, Animation")]

    [Serializable]
    public class InstructionCharacterPlayReactiveGesture : Instruction
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|

        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        [SerializeField] private ReactiveGesture m_ReactiveGesture;

        [Space] 
        [SerializeField] private bool m_WaitToComplete = true;

        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public override string Title => $"Play Gesture: {(m_ReactiveGesture ? m_ReactiveGesture.name : "(none)")}";

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected override async Task Run(Args args)
        {
            if (m_ReactiveGesture == null) return;
            
            var character = m_Character.Get<Character>(args);
            if (character == null) return;

            var task = character.PlayGesture(m_ReactiveGesture, args);

            if (m_WaitToComplete) await task;
        }

        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}