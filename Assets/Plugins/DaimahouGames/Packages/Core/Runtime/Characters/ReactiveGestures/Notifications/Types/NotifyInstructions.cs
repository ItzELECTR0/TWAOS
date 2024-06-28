using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace DaimahouGames.Runtime.Characters
{
    [Title("Instructions")]
    [Category("Instructions")]
    
    [Image(typeof(IconInstructions), ColorTheme.Type.Blue)]
    [Serializable]
    public class AnimNotifyInstructions : TNotify
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|

        [SerializeField] private string m_Descriptor;
        [SerializeField] protected InstructionList m_Instructions = new();
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public override string SubTitle => string.IsNullOrEmpty(m_Descriptor) ? "Instructions" : m_Descriptor;
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected override Task Trigger(Character character)
        {
            return m_Instructions.Run(new Args(character.gameObject));
        }
        
        // ※  Protected Methods: ------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}