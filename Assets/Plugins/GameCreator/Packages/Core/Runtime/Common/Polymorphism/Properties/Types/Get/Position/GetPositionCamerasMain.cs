using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Main Camera")]
    [Category("Cameras/Main Camera")]
    
    [Image(typeof(IconCamera), ColorTheme.Type.Green)]
    [Description("Returns the position of the Main Camera object")]

    [Serializable]
    public class GetPositionCamerasMain : PropertyTypeGetPosition
    {
        public override Vector3 Get(Args args)
        {
            Transform transform = ShortcutMainCamera.Transform;
            return transform != null ? transform.position : default;
        }
        
        public override Vector3 Get(GameObject gameObject)
        {
            Transform transform = ShortcutMainCamera.Transform;
            return transform != null ? transform.position : default;
        }

        public static PropertyGetPosition Create => new PropertyGetPosition(
            new GetPositionCamerasMain()
        );

        public override string String => "Main Camera";
    }
}