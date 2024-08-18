using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Enable Head Bobbing")]
    [Category("Cameras/Shots/Head Bobbing/Enable Head Bobbing")]
    [Description("Toggles the active state of a Camera Shot's Head Bobbing system")]

    [Parameter("Active", "The next state")]
    [Keywords("Cameras", "Disable", "Activate", "Deactivate", "Bool", "Toggle", "Off", "On")]

    [Serializable]
    public class InstructionShotHeadBobbingEnable : TInstructionShotHeadBobbing
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetBool m_Active = new PropertyGetBool(true);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[Head Bobbing] to {this.m_Active}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemHeadBobbing shotSystem = this.GetShotSystem<ShotSystemHeadBobbing>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.IsActive = this.m_Active.Get(args);
            
            return DefaultResult;
        }
    }
}