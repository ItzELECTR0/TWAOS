using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Smooth Time")]
    [Category("Cameras/Shots/First Person/Change Smooth Time")]
    [Description("Changes the maximum rotation (up and down) allowed")]

    [Parameter("Smooth Time", "How smooth the camera operates when rotating")]

    [Serializable]
    public class InstructionShotFirstPersonSmoothTime : TInstructionShotFirstPerson
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetDecimal m_SmoothTime = GetDecimalDecimal.Create(0.1f);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[First Person] Smooth Time = {this.m_SmoothTime}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemFirstPerson shotSystem = this.GetShotSystem<ShotSystemFirstPerson>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.SmoothTime = (float) this.m_SmoothTime.Get(args);
            
            return DefaultResult;
        }
    }
}