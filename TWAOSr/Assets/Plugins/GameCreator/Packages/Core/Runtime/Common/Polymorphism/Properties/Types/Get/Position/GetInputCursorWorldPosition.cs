using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Cursor World Position")]
    [Category("Input/Cursor World Position")]
    
    [Image(typeof(IconCursor), ColorTheme.Type.Green)]
    [Description("Returns the raw position of the Cursor in World-space")]

    [Serializable]
    public class GetInputCursorWorldPosition : PropertyTypeGetPosition
    {
        public override Vector3 Get(Args args)
        {
            Vector2 point = Mouse.current.position.ReadValue();
            Camera camera = ShortcutMainCamera.Get<Camera>();

            return camera != null ? camera.ScreenToWorldPoint(point) : default;
        }
        
        public override Vector3 Get(GameObject gameObject)
        {
            Vector2 point = Mouse.current.position.ReadValue();
            Camera camera = ShortcutMainCamera.Get<Camera>();

            return camera != null ? camera.ScreenToWorldPoint(point) : default;
        }

        public static PropertyGetPosition Create => new PropertyGetPosition(
            new GetInputCursorWorldPosition()
        );

        public override string String => "Cursor";
    }
}