using System;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Version(0, 1, 1)]
    
    [Title("Custom Effects")]
    [Category("Custom/Custom Effects")]
    
    [Keywords("Custom", "Generic")]

    [Image(typeof(IconInstructions), ColorTheme.Type.Teal)]
    
    [Serializable]
    public class AbilityEffectInstructions : AbilityEffect
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|

        [SerializeField] private string m_Descriptor;
        [SerializeField] private InstructionList m_Instructions;

        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|
        
        protected override string Summary => string.Format("{0}", 
            string.IsNullOrEmpty(m_Descriptor) ? $"Run {m_Instructions.Length} instructions" : m_Descriptor
        );
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected override void Apply_Internal(ExtendedArgs args)
        {
            m_Instructions.Run(args);
        }
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}