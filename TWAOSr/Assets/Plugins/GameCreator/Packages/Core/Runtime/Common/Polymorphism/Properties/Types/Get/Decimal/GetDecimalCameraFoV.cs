using System;
using GameCreator.Runtime.Cameras;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Field of View")]
    [Category("Camera/Field of View")]
    
    [Image(typeof(IconCamera), ColorTheme.Type.Yellow)]
    [Description("The targeted camera's field of view")]

    [Keywords("FOV", "Aperture", "Angle", "Cone", "View")]
    
    [Serializable]
    public class GetDecimalCameraFoV : PropertyTypeGetDecimal
    {
        [SerializeField] private PropertyGetGameObject m_Camera = GetGameObjectCameraMain.Create;
        
        public override double Get(Args args) => this.GetValue(args);

        private double GetValue(Args args)
        {
            Camera camera = this.m_Camera.Get<Camera>(args);
            return camera != null ? camera.fieldOfView : 0;
        }

        public static PropertyGetDecimal Create => new PropertyGetDecimal(
            new GetDecimalCameraFoV()
        );

        public override string String => $"{m_Camera} FoV";
    }
}