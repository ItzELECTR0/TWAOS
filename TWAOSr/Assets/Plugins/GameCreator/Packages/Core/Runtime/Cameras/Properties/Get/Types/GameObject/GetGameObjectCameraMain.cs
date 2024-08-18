using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Cameras
{
    [Title("Main Camera")]
    [Category("Cameras/Main Camera")]
    
    [Image(typeof(IconCamera), ColorTheme.Type.Green, typeof(OverlayDot))]
    [Description("Camera that represents the Main Camera")]

    [Serializable]
    public class GetGameObjectCameraMain : PropertyTypeGetGameObject
    {
        public override GameObject Get(Args args)
        {
            return ShortcutMainCamera.Instance != null 
                ? ShortcutMainCamera.Instance
                : null;
        }

        public override GameObject Get(GameObject gameObject)
        {
            return ShortcutMainCamera.Instance != null 
                ? ShortcutMainCamera.Instance
                : null;
        }

        public static PropertyGetGameObject Create => new PropertyGetGameObject(
            new GetGameObjectCameraMain()
        );

        public override string String => "Main Camera";
    }
}