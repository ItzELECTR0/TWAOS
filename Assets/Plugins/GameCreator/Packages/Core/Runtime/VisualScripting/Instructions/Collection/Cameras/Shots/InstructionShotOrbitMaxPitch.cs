using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Max Pitch")]
    [Category("Cameras/Shots/Orbit/Change Max Pitch")]
    [Description("Changes the maximum rotation (up and down) allowed")]

    [Parameter("Max Pitch", "The amount the Shot is allowed to look up and down, in degrees")]

    [Serializable]
    public class InstructionShotOrbitMaxPitch : TInstructionShotOrbit
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetDecimal m_MaxPitch = GetDecimalDecimal.Create(60f);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[Orbit] Max Pitch = {this.m_MaxPitch}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemOrbit shotSystem = this.GetShotSystem<ShotSystemOrbit>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.MaxPitch = (float) this.m_MaxPitch.Get(args);
            
            return DefaultResult;
        }
    }
}