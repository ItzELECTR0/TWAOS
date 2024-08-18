using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Title("Main Shot")]
    [Category("Cameras/Main Shot")]
    
    [Image(typeof(IconCameraShot), ColorTheme.Type.Yellow, typeof(OverlayDot))]
    [Description("Reference to the current Main Camera Shot")]

    [Serializable]
    public class GetGameObjectShotMain : PropertyTypeGetGameObject
    {
        public override GameObject Get(Args args)
        {
            return ShortcutMainShot.Instance;
        }

        public static PropertyGetGameObject Create => new PropertyGetGameObject(
            new GetGameObjectShotMain()
        );

        public override string String => "Main Shot";
    }
}