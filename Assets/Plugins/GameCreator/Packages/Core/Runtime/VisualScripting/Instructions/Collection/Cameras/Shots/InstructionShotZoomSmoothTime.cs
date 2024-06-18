using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Smooth Time")]
    [Category("Cameras/Shots/Zoom/Change Smooth Time")]
    [Description("Changes how smooth the zoom responds to input")]

    [Parameter("Smooth Time", "How smooth is the zoom transition")]

    [Serializable]
    public class InstructionShotZoomSmoothTime : TInstructionShotZoom
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetDecimal m_SmoothTime = GetDecimalDecimal.Create(0.1f);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[Zoom] Smooth Time = {this.m_SmoothTime}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemZoom shotSystem = this.GetShotSystem<ShotSystemZoom>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.SmoothTime = (float) this.m_SmoothTime.Get(args);
            
            return DefaultResult;
        }
    }
}