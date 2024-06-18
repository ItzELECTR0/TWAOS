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
    [Category("Cameras/Shots/First Person/Change Target")]
    [Description("Changes the targeted game object to view from")]

    [Parameter("Target", "The new target")]
    [Keywords("Cameras", "Track", "View")]

    [Serializable]
    public class InstructionShotFirstPersonTarget : TInstructionShotFirstPerson
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetGameObject m_Target = GetGameObjectPlayer.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot}[First Person] Target = {this.m_Target}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemFirstPerson shotSystem = this.GetShotSystem<ShotSystemFirstPerson>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.Target = this.m_Target.Get(args);
            
            return DefaultResult;
        }
    }
}