using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Max Pitch")]
    [Category("Cameras/Shots/First Person/Change Max Pitch")]
    [Description("Changes the maximum rotation (up and down) allowed")]

    [Parameter("Max Pitch", "The amount the Shot is allowed to look up and down, in degrees")]

    [Serializable]
    public class InstructionShotFirstPersonMaxPitch : TInstructionShotFirstPerson
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetDecimal m_MaxPitch = GetDecimalDecimal.Create(60f);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[First Person] Max Pitch = {this.m_MaxPitch}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemFirstPerson shotSystem = this.GetShotSystem<ShotSystemFirstPerson>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.MaxPitch = (float) this.m_MaxPitch.Get(args);
            
            return DefaultResult;
        }
    }
}