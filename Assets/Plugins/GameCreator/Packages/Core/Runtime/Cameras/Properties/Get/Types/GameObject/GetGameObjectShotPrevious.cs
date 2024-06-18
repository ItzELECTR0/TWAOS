using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Title("Previous Shot")]
    [Category("Cameras/Previous Shot")]
    
    [Image(typeof(IconCameraShot), ColorTheme.Type.Yellow, typeof(OverlayArrowLeft))]
    [Description("Reference to the previous Camera Shot used by a Camera")]

    [Serializable]
    public class GetGameObjectShotPrevious : PropertyTypeGetGameObject
    {
        [SerializeField]
        protected PropertyGetGameObject m_Camera = GetGameObjectCameraMain.Create;

        public override GameObject Get(Args args)
        {
            TCamera camera = this.m_Camera.Get<TCamera>(args);
            ShotCamera shot = camera != null ? camera.Transition.PreviousShotCamera : null;

            return shot != null ? shot.gameObject : null;
        }

        public static PropertyGetGameObject Create => new PropertyGetGameObject(
            new GetGameObjectShotPrevious()
        );

        public override string String => $"{this.m_Camera} Previous Shot";
    }
}