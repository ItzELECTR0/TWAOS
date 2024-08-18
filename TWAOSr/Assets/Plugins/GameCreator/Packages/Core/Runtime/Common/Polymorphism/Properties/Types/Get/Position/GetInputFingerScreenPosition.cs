using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Cursor Screen Position")]
    [Category("Input/Cursor Screen Position")]
    
    [Image(typeof(IconFinger), ColorTheme.Type.Yellow)]
    [Description("Returns the raw position of the Finger in Screen-space")]

    [Serializable]
    public class GetInputFingerScreenPosition : PropertyTypeGetPosition
    {
        public override Vector3 Get(Args args)
        {
            return Touchscreen.current.position.ReadValue();
        }
        
        public override Vector3 Get(GameObject gameObject)
        {
            return Touchscreen.current.position.ReadValue();
        }

        public static PropertyGetPosition Create => new PropertyGetPosition(
            new GetInputFingerScreenPosition()
        );

        public override string String => "Finger";
    }
}