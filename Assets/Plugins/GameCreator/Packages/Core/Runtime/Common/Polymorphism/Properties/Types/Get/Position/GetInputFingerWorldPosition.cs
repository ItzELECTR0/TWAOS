using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Finger World Position")]
    [Category("Input/Finger World Position")]
    
    [Image(typeof(IconFinger), ColorTheme.Type.Green)]
    [Description("Returns the raw position of the Finger in World-space")]

    [Serializable]
    public class GetInputFingerWorldPosition : PropertyTypeGetPosition
    {
        public override Vector3 Get(Args args)
        {
            Vector2 point = Touchscreen.current.position.ReadValue();
            Camera camera = ShortcutMainCamera.Get<Camera>();

            return camera != null ? camera.ScreenToWorldPoint(point) : default;
        }
        
        public override Vector3 Get(GameObject gameObject)
        {
            Vector2 point = Touchscreen.current.position.ReadValue();
            Camera camera = ShortcutMainCamera.Get<Camera>();

            return camera != null ? camera.ScreenToWorldPoint(point) : default;
        }

        public static PropertyGetPosition Create => new PropertyGetPosition(
            new GetInputFingerWorldPosition()
        );

        public override string String => "Finger";
    }
}