using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Capsule")]
    [Category("Capsule")]
    
    [Image(typeof(IconCapsuleSolid), ColorTheme.Type.Green)]
    [Description("Use a Capsule volume")]
    
    [Serializable]
    public class VolumeCapsule : TVolume
    {
        private const int SEGMENTS = 24;
        
        // ENUMS: ---------------------------------------------------------------------------------
        
        public enum Direction
        {
            AxisX = 0,
            AxisY = 1,
            AxisZ = 2
        }
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private Vector3 m_Center = Vector3.zero;
        [SerializeField] private float m_Height = 1f;
        [SerializeField] private float m_Radius = 0.1f;
        [SerializeField] private Direction m_Direction = Direction.AxisX;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public VolumeCapsule() : base()
        { }

        public VolumeCapsule(
            HumanBodyBones humanBone,
            float weight,
            IJoint joint,
            Vector3 center,
            float height, 
            float radius, 
            Direction direction)
            : base(humanBone, weight, joint)
        {
            this.m_Center = center;
            this.m_Height = height;
            this.m_Radius = radius;
            this.m_Direction = direction;
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------
        
        protected override Collider SetupCollider(GameObject bone, Skeleton skeleton)
        {
            CapsuleCollider collider = bone.Get<CapsuleCollider>();
            if (collider == null) collider = bone.Add<CapsuleCollider>();

            collider.enabled = false;
            collider.center = this.m_Center;
            collider.height = this.m_Height;
            collider.radius = this.m_Radius;
            collider.direction = (int)this.m_Direction;

            return collider;
        }
        
        // DRAW GIZMOS: ---------------------------------------------------------------------------

        protected override void DrawGizmos(Transform bone, Volumes.Display display)
        {
            switch (display)
            {
                case Volumes.Display.Outline:
                    GizmosExtension.CapsuleWire(
                        bone.TransformPoint(this.m_Center),
                        bone.rotation,
                        this.m_Radius * this.GetBoneScale(bone),
                        this.m_Height * this.GetBoneScale(bone),
                        SEGMENTS,
                        (int) this.m_Direction
                    );
                    break;
                
                case Volumes.Display.Solid:
                    GizmosExtension.Capsule(
                        bone.TransformPoint(this.m_Center),
                        bone.rotation,
                        this.m_Radius * this.GetBoneScale(bone),
                        this.m_Height * this.GetBoneScale(bone),
                        SEGMENTS,
                        (int) this.m_Direction
                    );
                    break;
            }
        }
    }
}