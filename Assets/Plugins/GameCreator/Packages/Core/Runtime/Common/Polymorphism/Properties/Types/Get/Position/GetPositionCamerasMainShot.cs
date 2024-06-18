using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Main Shot")]
    [Category("Cameras/Main Shot")]
    
    [Image(typeof(IconCameraShot), ColorTheme.Type.Yellow)]
    [Description("Returns the position of the main Camera Shot object")]

    [Serializable]
    public class GetPositionCamerasMainShot : PropertyTypeGetPosition
    {
        public override Vector3 Get(Args args)
        {
            Transform transform = ShortcutMainShot.Transform;
            return transform != null ? transform.position : default;
        }
        
        public override Vector3 Get(GameObject gameObject)
        {
            Transform transform = ShortcutMainShot.Transform;
            return transform != null ? transform.position : default;
        }

        public static PropertyGetPosition Create => new PropertyGetPosition(
            new GetPositionCamerasMainShot()
        );

        public override string String => "Main Shot";
    }
}