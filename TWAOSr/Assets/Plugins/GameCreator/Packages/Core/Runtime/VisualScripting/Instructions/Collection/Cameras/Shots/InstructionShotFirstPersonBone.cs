using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Bone")]
    [Category("Cameras/Shots/First Person/Change Bone")]
    [Description("Changes the Bone mount of the targeted object")]

    [Parameter("Bone", "The new bone of the character")]

    [Serializable]
    public class InstructionShotFirstPersonBone : TInstructionShotFirstPerson
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private Bone m_Bone = new Bone(HumanBodyBones.Head);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot} = {this.m_Bone}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotSystemFirstPerson shotSystem = this.GetShotSystem<ShotSystemFirstPerson>(args);
            
            if (shotSystem == null) return DefaultResult;
            shotSystem.Bone = this.m_Bone;
            
            return DefaultResult;
        }
    }
}