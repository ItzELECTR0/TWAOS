using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Overlap Box 3D")]
    [Category("Physics 3D/Overlap Box 3D")]
    
    [Description("Captures all colliders caught inside a box")]
    [Image(typeof(IconCubeOutline), ColorTheme.Type.Green, typeof(OverlayPhysics))]

    [Parameter(
        "Center", 
        "The center of the box"
    )]
    
    [Parameter(
        "Half Extents", 
        "Half of the size of the box in each axis"
    )]
    
    [Parameter(
        "Rotation", 
        "The rotation of the box in world space"
    )]
    
    [Keywords("Cube")]
    [Serializable]
    public class InstructionPhysics3DOverlapBox : TInstructionPhysics3DOverlap
    {
        [SerializeField] 
        private PropertyGetPosition m_Center = GetPositionCharacter.Create;

        [SerializeField]
        private PropertyGetDirection m_HalfExtents = GetDirectionVector.Create(Vector3.one);
        
        [SerializeField]
        private PropertyGetRotation m_Rotation = GetRotationIdentity.Create;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Overlap Box at {this.m_Center}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override int GetColliders(Collider[] colliders, Args args)
        {
            Vector3 center = this.m_Center.Get(args);
            Vector3 halfExtents = this.m_HalfExtents.Get(args);
            Quaternion rotation = this.m_Rotation.Get(args);

            return Physics.OverlapBoxNonAlloc(
                center, halfExtents, 
                colliders, rotation,
                this.m_LayerMask, 
                QueryTriggerInteraction.Ignore
            );
        }
    }
}