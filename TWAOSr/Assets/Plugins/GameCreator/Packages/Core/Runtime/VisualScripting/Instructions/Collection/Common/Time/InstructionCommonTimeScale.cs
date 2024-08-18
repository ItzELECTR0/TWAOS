using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Time Scale")]
    [Description("Changes the Time Scale of the game")]
    
    [Example("Setting a Time Scale of 0 will freeze the game. Useful for pausing the game")]
    [Example(
        "The resulting Time Scale will be equal to the lowest time scale value between all " +
        "Layers. For example, if the Time Scale with Layer = 0 has a value of 0.5 (which makes " +
        "characters move in slow motion), and another Time Scale with Layer = 1 with a value of 0, " +
        "the resulting Time Scale will be 0"
    )]

    [Category("Time/Time Scale")]

    [Parameter(
        "Time Scale",
        "The scale at which time passes. This can be used for slow motion effects"
    )]
    
    [Parameter(
        "Blend Time", 
        "How long it takes to transition from the current time scale to the new one"
    )]
    
    [Parameter(
        "Layer", 
        "Any time scale values using the same Layer is overwritten by this one."
    )]

    [Keywords("Slow", "Motion", "Bullet", "Time", "Matrix")]
    [Image(typeof(IconClock), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionCommonTimeScale : Instruction
    {
        [SerializeField]
        private PropertyGetDecimal m_TimeScale = new PropertyGetDecimal(1f);
        
        [SerializeField] private float m_TransitionDuration = 0.25f;
        [SerializeField] private int m_Layer;

        public override string Title => $"Set Time Scale to {this.m_TimeScale}";

        protected override Task Run(Args args)
        {
            float timeScale = (float) this.m_TimeScale.Get(args);
            TimeManager.Instance.SetSmoothTimeScale(
                timeScale,
                this.m_TransitionDuration,
                this.m_Layer
            );

            return DefaultResult;
        }
    }
}
