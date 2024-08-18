using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Field of View")]
    [Description("Changes the camera field of view")]

    [Category("Cameras/Properties/Change Field of View")]

    [Parameter("Camera", "The camera component whose property changes")]
    [Parameter("FoV", "The field of view of the camera, measured in degrees")]
    [Parameter("Duration", "The time in seconds, it takes for the camera to complete the change")]
    [Parameter("Easing", "The easing function used to transition")]

    [Keywords("Cameras", "Perspective", "FOV", "3D")]
    [Image(typeof(IconCamera), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class InstructionCameraFOV : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetGameObject m_Camera = GetGameObjectCameraMain.Create;
        
        [SerializeField] private PropertyGetDecimal m_FieldOfView = new PropertyGetDecimal(60f);

        [SerializeField] private PropertyGetDecimal m_Duration = new PropertyGetDecimal(1f);
        [SerializeField] private Easing.Type m_Easing = Easing.Type.QuadInOut;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Change Field of View to {this.m_FieldOfView}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            TCamera camera = this.m_Camera.Get<TCamera>(args);
            if (camera == null) return DefaultResult;

            float fov = (float) this.m_FieldOfView.Get(args);
            float duration = (float) this.m_Duration.Get(args);

            camera.Viewport.SetFieldOfView(fov, duration, this.m_Easing);
            return DefaultResult;
        }
    }
}