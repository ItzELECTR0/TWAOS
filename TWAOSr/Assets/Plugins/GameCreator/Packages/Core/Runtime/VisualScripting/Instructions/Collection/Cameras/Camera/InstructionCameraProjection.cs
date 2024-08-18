using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Projection")]
    [Description("Changes the camera projection to either Perspective or Orthographic")]

    [Category("Cameras/Properties/Change Projection")]

    [Parameter("Camera", "The camera component whose property changes")]
    [Parameter("Projection", "Whether to change to Orthographic or Perspective mode")]

    [Keywords("Cameras", "Orthographic", "Perspective", "3D", "2D")]
    [Image(typeof(IconCamera), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class InstructionCameraProjection : Instruction
    {
        private enum Projection
        {
            Orthographic,
            Perspective
        }
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetGameObject m_Camera = GetGameObjectCameraMain.Create;

        [Space] 
        [SerializeField] private Projection m_Projection = Projection.Perspective;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Change Projection to {this.m_Projection}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            TCamera camera = this.m_Camera.Get<TCamera>(args);
            if (camera == null) return DefaultResult;

            bool isOrthographic = this.m_Projection == Projection.Orthographic;
            camera.Viewport.SetProjection(isOrthographic);
            
            return DefaultResult;
        }
    }
}