using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Sphere")]
    [Category("Sphere")]
    
    [Image(typeof(IconSphereSolid), ColorTheme.Type.Green)]
    [Description("Use a Spherical volume")]
    
    [Serializable]
    public class VolumeSphere : TVolume
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private Vector3 m_Center = Vector3.zero;
        [SerializeField] private float m_Radius = 0.1f;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public VolumeSphere() : base()
        { }

        public VolumeSphere(
            HumanBodyBones humanBone,
            float weight,
            IJoint joint,
            Vector3 center,
            float radius) 
            : base(humanBone, weight, joint)
        {
            this.m_Center = center;
            this.m_Radius = radius;
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------
        
        protected override Collider SetupCollider(GameObject bone, Skeleton skeleton)
        {
            SphereCollider collider = bone.Get<SphereCollider>();
            if (collider == null) collider = bone.Add<SphereCollider>();

            collider.enabled = false;
            collider.center = this.m_Center;
            collider.radius = this.m_Radius;

            return collider;
        }
        
        // DRAW GIZMOS: ---------------------------------------------------------------------------

        protected override void DrawGizmos(Transform bone, Volumes.Display display)
        {
            switch (display)
            {
                case Volumes.Display.Outline:
                    GizmosExtension.OctahedronWire(
                        bone.TransformPoint(this.m_Center),
                        bone.rotation,
                        this.m_Radius * this.GetBoneScale(bone)
                    );
                    break;
                
                case Volumes.Display.Solid:
                    GizmosExtension.Octahedron(
                        bone.TransformPoint(this.m_Center),
                        bone.rotation,
                        this.m_Radius * this.GetBoneScale(bone)
                    );
                    break;
            }
        }
    }
}