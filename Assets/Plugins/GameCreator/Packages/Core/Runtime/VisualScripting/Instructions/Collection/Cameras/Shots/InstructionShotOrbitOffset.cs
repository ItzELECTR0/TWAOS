using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Offset")]
    [Category("Cameras/Shots/Orbit/Change Offset")]
    [Description("Changes the offset position of the targeted object to orbit")]

    [Parameter("Offset", "The new offset in self local coordinates")]
    [Keywords("Cameras", "Track", "View")]

    [Serializable]
    public class InstructionShotOrbitOffset : TInstructionShotOrbit
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetPosition m_Offset = GetPositionVector3.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[Orbit] Offset = {this.m_Offset}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemOrbit shotSystem = this.GetShotSystem<ShotSystemOrbit>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.Offset = this.m_Offset.Get(args);
            
            return DefaultResult;
        }
    }
}