using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Min Distance")]
    [Category("Cameras/Shots/Zoom/Change Min Distance")]
    [Description("Changes the targeted zoom level percentage")]

    [Parameter("Min Distance", "The minimum zoom distance between the target and the Shot")]

    [Serializable]
    public class InstructionShotZoomMinDistance : TInstructionShotZoom
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetDecimal m_MinDistance = GetDecimalDecimal.Create(1f);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[Zoom] Min Distance = {this.m_MinDistance}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemZoom shotSystem = this.GetShotSystem<ShotSystemZoom>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.MinDistance = (float) this.m_MinDistance.Get(args);
            
            return DefaultResult;
        }
    }
}