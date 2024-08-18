using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace GameCreator.Runtime.VisualScripting
{
    [Parameter("Button", "The Mouse button that is checked")]
    
    [Keywords("Cursor")]
    
    [Serializable]
    public abstract class TConditionMouse : Condition
    {
        protected enum Button
        {
            Left = MouseButton.Left,
            Right = MouseButton.Right,
            Middle = MouseButton.Middle,
            Forward = MouseButton.Forward,
            Back = MouseButton.Back
        }
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] protected Button m_Button = Button.Left;
    }
}
