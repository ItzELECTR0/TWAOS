using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Distance")]
    [Category("Cameras/Shots/Follow/Change Distance")]
    [Description("Changes the offset distance between the Shot and the targeted object")]

    [Parameter("Distance", "The new offset distance in world coordinates")]
    [Keywords("Cameras", "Track", "View")]

    [Serializable]
    public class InstructionShotFollowDistance : TInstructionShotFollow
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetPosition m_Offset = GetPositionVector3.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[Follow] Distance = {this.m_Offset}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemFollow shotSystem = this.GetShotSystem<ShotSystemFollow>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.Distance = this.m_Offset.Get(args);
            
            return DefaultResult;
        }
    }
}