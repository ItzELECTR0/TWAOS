using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Enable Look")]
    [Category("Cameras/Shots/Look/Enable Look")]
    [Description("Toggles the active state of a Camera Shot's Look system")]

    [Parameter("Active", "The next state")]
    [Keywords("Cameras", "Disable", "Activate", "Deactivate", "Bool", "Toggle", "Off", "On")]

    [Serializable]
    public class InstructionShotLookEnable : TInstructionShotLook
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetBool m_Active = new PropertyGetBool(true);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[Look] to {this.m_Active}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemLook shotSystem = this.GetShotSystem<ShotSystemLook>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.IsActive = this.m_Active.Get(args);
            
            return DefaultResult;
        }
    }
}