using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Offset")]
    [Category("Cameras/Shots/Lock On/Change Offset")]
    [Description("Changes the offset position of the targeted object")]

    [Parameter("Offset", "The new offset in self local coordinates")]
    [Keywords("Cameras", "Track", "View")]

    [Serializable]
    public class InstructionShotLockOnOffset : TInstructionShotLockOn
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetPosition m_Offset = GetPositionVector3.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[Lock On] Offset = {this.m_Offset}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemLockOn shotSystem = this.GetShotSystem<ShotSystemLockOn>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.Offset = this.m_Offset.Get(args);
            
            return DefaultResult;
        }
    }
}