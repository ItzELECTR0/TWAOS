using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters.IK
{
    [Title("Align Feet with Ground")]
    [Category("Align Feet with Ground")]
    [Image(typeof(IconFootprint), ColorTheme.Type.Green)]
    
    [Description(
        "IK system that allows the Character to correctly align their feet to uneven terrain. " +
        "It also avoids character's feet from penetrating the floor. Requires a humanoid character"
    )]
    
    [Serializable]
    public class RigFeetPlant : TRigAnimatorIK
    {
        // CONSTANTS: -----------------------------------------------------------------------------

        public const string RIG_NAME = "RigFeetPlant";

        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private float m_FootOffset;
        [SerializeField] private LayerMask m_FootMask = Physics.DefaultRaycastLayers;
        [SerializeField] private float m_SmoothTime = 0.25f;

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private FootPlant m_LimbFootL;
        [NonSerialized] private FootPlant m_LimbFootR;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => "Align Feet with Ground";
        
        public override string Name => RIG_NAME;
        
        public override bool RequiresHuman => true;
        public override bool DisableOnBusy => false;

        internal float FootOffset => m_FootOffset;
        internal LayerMask FootMask => this.m_FootMask;
        internal float SmoothTime => this.m_SmoothTime;

        // IMPLEMENT METHODS: ---------------------------------------------------------------------

        protected override void DoStartup(Character character)
        {
            this.m_LimbFootL = new FootPlant(HumanBodyBones.LeftFoot,  AvatarIKGoal.LeftFoot,  this, 0);
            this.m_LimbFootR = new FootPlant(HumanBodyBones.RightFoot, AvatarIKGoal.RightFoot, this, 1);

            base.DoStartup(character);
        }

        protected override void DoUpdate(Character character)
        {
            base.DoUpdate(character);

            this.m_LimbFootL.Update();
            this.m_LimbFootR.Update();
        }
    }
}