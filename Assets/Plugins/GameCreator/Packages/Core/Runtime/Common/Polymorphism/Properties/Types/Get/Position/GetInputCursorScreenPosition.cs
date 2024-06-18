using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Cursor Screen Position")]
    [Category("Input/Cursor Screen Position")]
    
    [Image(typeof(IconCursor), ColorTheme.Type.Yellow)]
    [Description("Returns the raw position of the Cursor in Screen-space")]

    [Serializable]
    public class GetInputCursorScreenPosition : PropertyTypeGetPosition
    {
        public override Vector3 Get(Args args)
        {
            return Mouse.current.position.ReadValue();
        }
        
        public override Vector3 Get(GameObject gameObject)
        {
            return Mouse.current.position.ReadValue();
        }

        public static PropertyGetPosition Create => new PropertyGetPosition(
            new GetInputCursorScreenPosition()
        );

        public override string String => "Cursor";
    }
}