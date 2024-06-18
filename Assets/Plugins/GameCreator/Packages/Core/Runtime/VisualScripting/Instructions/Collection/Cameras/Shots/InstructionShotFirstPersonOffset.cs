using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Offset")]
    [Category("Cameras/Shots/First Person/Change Offset")]
    [Description("Changes the offset position of the targeted object")]

    [Parameter("Offset", "The new offset in self local coordinates")]

    [Serializable]
    public class InstructionShotFirstPersonOffset : TInstructionShotFirstPerson
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetPosition m_Offset = GetPositionVector3.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[First Person] Offset = {this.m_Offset}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemFirstPerson shotSystem = this.GetShotSystem<ShotSystemFirstPerson>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.Offset = this.m_Offset.Get(args);
            
            return DefaultResult;
        }
    }
}