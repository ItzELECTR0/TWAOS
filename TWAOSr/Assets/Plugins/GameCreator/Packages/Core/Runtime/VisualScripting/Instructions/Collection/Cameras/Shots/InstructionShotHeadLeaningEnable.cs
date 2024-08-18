using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Enable Head Leaning")]
    [Category("Cameras/Shots/Head Leaning/Enable Head Leaning")]
    [Description("Toggles the active state of a Camera Shot's Head Leaning system")]

    [Parameter("Active", "The next state")]
    [Keywords("Cameras", "Disable", "Activate", "Deactivate", "Bool", "Toggle", "Off", "On")]

    [Serializable]
    public class InstructionShotHeadLeaningEnable : TInstructionShotHeadLeaning
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetBool m_Active = new PropertyGetBool(true);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[Head Leaning] to {this.m_Active}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemHeadLeaning shotSystem = this.GetShotSystem<ShotSystemHeadLeaning>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.IsActive = this.m_Active.Get(args);
            
            return DefaultResult;
        }
    }
}