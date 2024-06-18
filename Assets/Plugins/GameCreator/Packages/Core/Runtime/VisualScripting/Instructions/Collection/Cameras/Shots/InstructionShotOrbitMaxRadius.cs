using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Max Radius")]
    [Category("Cameras/Shots/Orbit/Change Max Radius")]
    [Description("Changes the maximum rotation (up and down) allowed")]

    [Parameter("Max Radius", "The amount the Shot is allowed to look up and down, in degrees")]

    [Serializable]
    public class InstructionShotOrbitMaxRadius : TInstructionShotOrbit
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetDecimal m_MaxRadius = GetDecimalDecimal.Create(5f);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[Orbit] Max Radius = {this.m_MaxRadius}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemOrbit shotSystem = this.GetShotSystem<ShotSystemOrbit>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.MaxRadius = (float) this.m_MaxRadius.Get(args);
            
            return DefaultResult;
        }
    }
}