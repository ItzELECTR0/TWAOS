using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Box")]
    [Category("Box")]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Green)]
    [Description("Use a Cubic volume")]
    
    [Serializable]
    public class VolumeBox : TVolume
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private Vector3 m_Center = Vector3.zero;
        [SerializeField] private Vector3 m_Size = Vector3.zero;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public VolumeBox() : base()
        { }

        public VolumeBox(
            HumanBodyBones humanBone,
            float weight,
            IJoint joint,
            Vector3 center,
            Vector3 size) 
            : base(humanBone, weight, joint)
        {
            this.m_Center = center;
            this.m_Size = size;
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected override Collider SetupCollider(GameObject bone, Skeleton skeleton)
        {
            BoxCollider collider = bone.Get<BoxCollider>();
            if (collider == null) collider = bone.Add<BoxCollider>();

            collider.enabled = false;
            collider.center = this.m_Center;
            collider.size = this.m_Size;

            return collider;
        }
        
        // DRAW GIZMOS: ---------------------------------------------------------------------------

        protected override void DrawGizmos(Transform bone, Volumes.Display display)
        {
            switch (display)
            {
                case Volumes.Display.Outline:
                    GizmosExtension.BoxWire(
                        bone.TransformPoint(this.m_Center),
                        bone.rotation,
                        Vector3.Scale(this.m_Size, bone.lossyScale)
                    );
                    break;
                
                case Volumes.Display.Solid:
                    GizmosExtension.Box(
                        bone.TransformPoint(this.m_Center),
                        bone.rotation,
                        Vector3.Scale(this.m_Size, bone.lossyScale)
                    );
                    break;
            }
        }
    }
}