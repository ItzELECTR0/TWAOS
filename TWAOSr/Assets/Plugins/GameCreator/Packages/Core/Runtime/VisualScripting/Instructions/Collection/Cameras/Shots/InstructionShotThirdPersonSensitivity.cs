using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Sensitivity")]
    [Category("Cameras/Shots/Third Person/Change Sensitivity")]
    [Description("Changes how sensitive the Shot reacts to input")]

    [Parameter("Sensitivity", "Input sensitivity for X and the Y axis")]

    [MovedFrom(
        true,
        "GameCreator.Runtime.VisualScripting",
        "GameCreator.Runtime.Core",
        "InstructionShotOrbitSensitivity"
    )]
    
    [Serializable]
    public class InstructionShotThirdPersonSensitivity : TInstructionShotThirdPerson
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField]
        private PropertyGetPosition m_Sensitivity = GetPositionVector3.Create(
            new Vector3(180f, 180f)
        );

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[Third Person] Sensitivity = {this.m_Sensitivity}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemThirdPerson shotSystem = this.GetShotSystem<ShotSystemThirdPerson>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.Sensitivity = this.m_Sensitivity.Get(args);
            
            return DefaultResult;
        }
    }
}