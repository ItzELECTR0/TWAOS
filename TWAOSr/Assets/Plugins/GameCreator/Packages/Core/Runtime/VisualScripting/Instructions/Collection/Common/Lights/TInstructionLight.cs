using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Parameter("Light", "The game object with a Light component")]
    [Parameter("Duration", "How long it takes to perform the transition")]
    [Parameter("Easing", "The change rate of the parameter over time")]
    [Parameter("Wait to Complete", "Whether to wait until the transition is finished")]
    
    [Keywords("Light", "Spot", "Sun", "Point", "Strength", "Burn", "Dark")]
    [Image(typeof(IconLight), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public abstract class TInstructionLight : Instruction
    {
        [SerializeField] protected PropertyGetGameObject m_Light = new PropertyGetGameObject();
        
        [Space]
        [SerializeField] protected Transition m_Transition = new Transition();
    }
}