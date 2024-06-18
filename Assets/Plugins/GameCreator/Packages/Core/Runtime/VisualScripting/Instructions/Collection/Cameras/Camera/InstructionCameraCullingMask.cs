using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Culling Mask")]
    [Description("Changes the camera culling mask")]

    [Category("Cameras/Properties/Change Culling Mask")]

    [Parameter("Camera", "The camera component whose property changes")]
    [Parameter("Culling Mask", "The mask the camera uses to discern which objects to render")]

    [Keywords("Cameras", "Render")]
    [Image(typeof(IconCamera), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class InstructionCameraCullingMask : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetGameObject m_Camera = GetGameObjectCameraMain.Create;

        [Space] 
        [SerializeField] private LayerMask m_CullingMask = Physics.DefaultRaycastLayers;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => "Change Culling Mask";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            TCamera camera = this.m_Camera.Get<TCamera>(args);
            if (camera == null) return DefaultResult;

            camera.Get<Camera>().cullingMask = this.m_CullingMask;
            return DefaultResult;
        }
    }
}