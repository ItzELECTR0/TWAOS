using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Alignment")]
    [Category("Cameras/Shots/Orbit/Change Alignment")]
    [Description("Changes whether and how the Shot aligns behind the targeted object")]

    [Parameter("Align with Target", "If the Shot should move behind the target after some idle time")]
    [Parameter("Delay", "If the Shot should move behind the target after some idle time")]
    [Parameter("Smooth Time", "The speed at which ")]

    [Serializable]
    public class InstructionShotOrbitAlignment : TInstructionShotOrbit
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private bool m_AlignWithTarget = true;
        [SerializeField] private float m_Delay = 3f;
        [SerializeField] private float m_SmoothTime = 5f;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => string.Format(
            "Set {0}[Orbit] Align = {1}", 
            this.m_Shot,
            this.m_AlignWithTarget ? "Yes" : "No"
        );

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemOrbit shotSystem = this.GetShotSystem<ShotSystemOrbit>(args);
            if (shotSystem == null) return DefaultResult;
            
            shotSystem.AlignWithTarget = this.m_AlignWithTarget;
            shotSystem.AlignDelay = this.m_Delay;
            shotSystem.AlignSmoothTime = this.m_SmoothTime;
            
            return DefaultResult;
        }
    }
}