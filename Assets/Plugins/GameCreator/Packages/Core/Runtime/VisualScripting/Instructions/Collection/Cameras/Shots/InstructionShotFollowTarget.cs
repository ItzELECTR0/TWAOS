using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Target")]
    [Category("Cameras/Shots/Follow/Change Target")]
    [Description("Changes the targeted game object to Follow")]

    [Parameter("Follow", "The new target to follow")]
    [Keywords("Cameras", "Track", "View")]

    [Serializable]
    public class InstructionShotFollowTarget : TInstructionShotFollow
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetGameObject m_Follow = GetGameObjectPlayer.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[Follow] Follow = {this.m_Follow}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemFollow shotSystem = this.GetShotSystem<ShotSystemFollow>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.Follow = this.m_Follow.Get<Transform>(args);
            
            return DefaultResult;
        }
    }
}