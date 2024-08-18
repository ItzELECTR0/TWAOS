using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Level Zoom")]
    [Category("Cameras/Shots/Zoom/Change Level Zoom")]
    [Description("Changes the targeted zoom level percentage")]

    [Parameter("Level", "The zoom level value between zero and one")]

    [Serializable]
    public class InstructionShotZoomValue : TInstructionShotZoom
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetDecimal m_Level = GetDecimalDecimal.Create(0.5f);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[Zoom] Level = {this.m_Level}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemZoom shotSystem = this.GetShotSystem<ShotSystemZoom>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.Level = (float) this.m_Level.Get(args);
            
            return DefaultResult;
        }
    }
}