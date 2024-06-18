using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters.IK
{
    [Title("Align Body with Ground")]
    [Category("Align Body with Ground")]
    [Image(typeof(IconFloorNormal), ColorTheme.Type.Green)]
    
    [Description("Aligns the entire model with the normal vector from the ground")]
    
    [Serializable]
    public class RigAlignGround : TRigAnimatorIK
    {
        // CONSTANTS: -----------------------------------------------------------------------------

        public const string RIG_NAME = "RigAlignGround";

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private AnimVector3 m_Normal;

        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private float m_SmoothTime = 0.25f;
        [SerializeField] private float m_MaxAngle = 35f;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => "Align with Ground";
        
        public override string Name => RIG_NAME;
        
        public override bool RequiresHuman => false;
        public override bool DisableOnBusy => false;

        // IMPLEMENT METHODS: ---------------------------------------------------------------------

        protected override void DoStartup(Character character)
        {
            this.m_Normal = new AnimVector3(Vector3.up, this.m_SmoothTime);
            base.DoStartup(character);
        }

        protected override void DoUpdate(Character character)
        {
            base.DoUpdate(character);
            
            Vector3 normal = character.transform.InverseTransformDirection(
                character.Driver.FloorNormal
            );
            
            float delta = character.Time.DeltaTime;
            this.m_Normal.UpdateWithDelta(
                character.Driver.IsGrounded && normal.magnitude >= 0.5f
                    ? normal
                    : Vector3.up, 
                delta
            );
            
            Vector3 direction = Vector3.MoveTowards(
                Vector3.up, 
                this.m_Normal.Current, 
                this.m_MaxAngle
            );

            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, direction);
            
            character.Animim.Rotation = rotation;
            character.Animim.ApplyMannequinRotation();
        }
    }
}