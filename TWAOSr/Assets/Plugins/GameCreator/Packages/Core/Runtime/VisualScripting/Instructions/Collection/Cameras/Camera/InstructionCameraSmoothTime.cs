using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Smooth Time")]
    [Description("Changes the camera Smooth Time")]

    [Category("Cameras/Properties/Change Smooth Time")]

    [Parameter("Camera", "The camera component whose property changes")]
    [Parameter("Smooth Position", "The new smooth value for translation")]
    [Parameter("Smooth Rotation", "The new smooth value for rotation")]

    [Keywords("Cameras")]
    [Image(typeof(IconCamera), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class InstructionCameraSmoothTime : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetGameObject m_Camera = GetGameObjectCameraMain.Create;
        
        [SerializeField] private PropertyGetDecimal m_SmoothPosition = new PropertyGetDecimal(0.1f);
        [SerializeField] private PropertyGetDecimal m_SmoothRotation = new PropertyGetDecimal(0.1f);

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Change Smooth of {this.m_Camera}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            TCamera camera = this.m_Camera.Get<TCamera>(args);
            if (camera == null) return DefaultResult;

            camera.Transition.SmoothTimePosition = (float) this.m_SmoothPosition.Get(args);
            camera.Transition.SmoothTimeRotation = (float) this.m_SmoothRotation.Get(args);
            
            return DefaultResult;
        }
    }
}