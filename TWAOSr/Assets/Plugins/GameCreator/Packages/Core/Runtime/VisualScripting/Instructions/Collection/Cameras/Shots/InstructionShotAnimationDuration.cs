using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Duration")]
    [Category("Cameras/Shots/Animation/Change Duration")]
    [Description("Changes the duration it takes for the Animation shot to complete")]

    [Parameter("Duration", "The new duration in seconds")]
    [Keywords("Cameras", "Track", "View")]

    [Serializable]
    public class InstructionShotAnimationDuration : TInstructionShotAnimation
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetDecimal m_Duration = GetDecimalDecimal.Create(5f);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[Animation] Duration = {this.m_Duration}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemAnimation shotSystem = this.GetShotSystem<ShotSystemAnimation>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.Duration = (float) this.m_Duration.Get(args);
            
            return DefaultResult;
        }
    }
}