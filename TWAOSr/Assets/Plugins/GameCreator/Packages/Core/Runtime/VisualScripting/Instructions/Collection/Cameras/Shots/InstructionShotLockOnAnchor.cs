using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Anchor")]
    [Category("Cameras/Shots/Lock On/Change Anchor")]
    [Description("Changes the targeted game object to Lock On")]

    [Parameter("Anchor", "The new target to Anchor onto")]
    [Keywords("Cameras", "Track", "View")]

    [Serializable]
    public class InstructionShotLockOnAnchor : TInstructionShotLockOn
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetGameObject m_Anchor = GetGameObjectPlayer.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[Lock On] Anchor = {this.m_Anchor}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemLockOn shotSystem = this.GetShotSystem<ShotSystemLockOn>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.Anchor = this.m_Anchor.Get(args);
            
            return DefaultResult;
        }
    }
}