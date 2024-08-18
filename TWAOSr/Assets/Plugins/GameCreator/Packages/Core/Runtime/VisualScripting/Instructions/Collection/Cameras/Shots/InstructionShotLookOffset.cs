using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Offset")]
    [Category("Cameras/Shots/Look/Change Offset")]
    [Description("Changes the offset position of the targeted object")]

    [Parameter("Offset", "The new offset in self local coordinates")]
    [Keywords("Cameras", "Track", "View")]

    [Serializable]
    public class InstructionShotLookOffset : TInstructionShotLook
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetPosition m_Offset = GetPositionVector3.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[Look] Offset = {this.m_Offset}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemLook shotSystem = this.GetShotSystem<ShotSystemLook>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.Offset = this.m_Offset.Get(args);
            
            return DefaultResult;
        }
    }
}