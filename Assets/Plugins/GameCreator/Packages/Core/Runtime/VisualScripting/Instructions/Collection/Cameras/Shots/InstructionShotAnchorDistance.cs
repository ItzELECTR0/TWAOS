using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Distance")]
    [Category("Cameras/Shots/Anchor/Change Distance")]
    [Description("Changes the anchored position the Shot sits relative to the target")]

    [Parameter("Distance", "The new distance relative to the target in local coordinates")]
    [Keywords("Cameras", "View")]

    [Serializable]
    public class InstructionShotAnchorDistance : TInstructionShotAnchor
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetPosition m_Distance = GetPositionVector3.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[Anchor] Distance = {this.m_Distance}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemAnchor shotSystem = this.GetShotSystem<ShotSystemAnchor>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.Distance = this.m_Distance.Get(args);
            
            return DefaultResult;
        }
    }
}