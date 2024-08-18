using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Revert to Previous Shot")]
    [Description("Reverts the active Shot of a particular camera to the previous one")]

    [Category("Cameras/Revert to previous Shot")]

    [Parameter("Camera", "The target camera component")]
    [Parameter("Duration", "How long it takes to transition to the new Shot, in seconds")]

    [Keywords("Cameras", "Render", "Switch", "Move")]
    [Image(typeof(IconCameraShot), ColorTheme.Type.Yellow, typeof(OverlayArrowLeft))]
    
    [Serializable]
    public class InstructionCameraShotRevertPrevious : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetGameObject m_Camera = GetGameObjectCameraMain.Create;

        [Space] 
        [SerializeField] private float m_Duration = 0f;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Revert to {this.m_Camera}'s previous Shot";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            TCamera camera = this.m_Camera.Get<TCamera>(args);

            if (camera == null) return DefaultResult;
            camera.Transition.ChangeToPreviousShot(this.m_Duration);
            
            return DefaultResult;
        }
    }
}