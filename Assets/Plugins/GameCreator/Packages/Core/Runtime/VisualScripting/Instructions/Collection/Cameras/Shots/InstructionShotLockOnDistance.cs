using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Distance")]
    [Category("Cameras/Shots/Lock On/Change Distance")]
    [Description("Changes the distance from the anchor point")]

    [Parameter("Distance", "The new distance in self local coordinates")]
    [Keywords("Cameras", "Track", "View")]

    [Serializable]
    public class InstructionShotLockOnDistance : TInstructionShotLockOn
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetDecimal m_Distance = GetDecimalDecimal.Create(5f);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[Lock On] Distance = {this.m_Distance}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemLockOn shotSystem = this.GetShotSystem<ShotSystemLockOn>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.Distance = (float) this.m_Distance.Get(args);
            
            return DefaultResult;
        }
    }
}