using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Smooth Time")]
    [Category("Cameras/Shots/Orbit/Change Smooth Time")]
    [Description("Changes how smooth the orbit responds to input")]

    [Parameter("Smooth Time", "How smooth is the orbital translation")]

    [Serializable]
    public class InstructionShotOrbitSmoothTime : TInstructionShotOrbit
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetDecimal m_SmoothTime = GetDecimalDecimal.Create(0.1f);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[Orbit] Smooth Time = {this.m_SmoothTime}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemOrbit shotSystem = this.GetShotSystem<ShotSystemOrbit>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.SmoothTime = (float) this.m_SmoothTime.Get(args);
            
            return DefaultResult;
        }
    }
}