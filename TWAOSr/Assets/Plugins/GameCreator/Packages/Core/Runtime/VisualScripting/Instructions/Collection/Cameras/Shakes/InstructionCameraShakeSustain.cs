using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Shake Camera Sustain")]
    [Description("Starts shaking the camera until the effect is manually turned off")]

    [Category("Cameras/Shakes/Shake Camera Sustain")]

    [Parameter("Camera", "The camera that receives the sustain shake effect")]
    [Parameter("Delay", "Amount of time in seconds before the shake effect starts")]
    [Parameter("Transition", "Amount of seconds the shake effect takes to blend in")]
    
    [Parameter("Shake Position", "Whether the shake affects the position of the camera")]
    [Parameter("Shake Rotation", "Whether the shake affects the rotation of the camera")]
    [Parameter("Magnitude", "The maximum amount the camera displaces from its position")]
    [Parameter("Roughness", "Frequency or how violently the camera shakes")]
    
    [Parameter("Transform", "[Optional] Defines the origin of the shake")]
    [Parameter("Radius", "[Optional] Distance from the origin that the shake starts to fall-off")]

    [Keywords("Cameras", "Animation", "Animate", "Shake", "Wave", "Play")]
    [Image(typeof(IconCameraShake), ColorTheme.Type.Green)]
    
    [Serializable]
    public class InstructionCameraShakeSustain : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Camera = GetGameObjectCameraMain.Create;

        [Space]
        [SerializeField] private int m_Layer = 0;
        [SerializeField] private float m_Delay = 0f;
        [SerializeField] private float m_Transition = 0.5f;
        
        [Space] 
        [SerializeField] private ShakeEffect m_ShakeEffect = ShakeEffect.Create;

        public override string Title => 
            $"Sustain shake {this.m_Camera} in layer {this.m_Layer}";

        protected override Task Run(Args args)
        {
            TCamera camera = this.m_Camera.Get<TCamera>(args);
            if (camera == null) return DefaultResult;
            
            camera.AddSustainShake(
                this.m_Layer,
                this.m_Delay, 
                this.m_Transition, 
                this.m_ShakeEffect
            );
            
            return DefaultResult;
        }
    }
}