using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Check Box 3D")]
    [Description("Returns true if casting a 3D box at a position collides with something")]

    [Category("Physics/Check Box 3D")]
    
    [Parameter("Position", "The scene position where the box's center is cast")]
    [Parameter("Rotation", "The rotation of the cube cast in world space")]
    [Parameter("Half Extents", "Half size of the cube that extents along its local axis")]
    [Parameter("Layer Mask", "A bitmask that skips any objects that don't belong to the list")]
    
    [Example(
        "Note that this Instruction uses Unity's 3D physics engine. " +
        "It won't collide with any 2D objects"
    )]

    [Keywords("Check", "Collide", "Touch", "Suit", "Square", "Cube", "3D")]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Green)]
    [Serializable]
    public class ConditionPhysicsCheckBox3D : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetPosition m_Position = GetPositionCharacter.Create;
        [SerializeField] private PropertyGetRotation m_Rotation = GetRotationCharacter.Create;
        
        [SerializeField] private Vector3 m_HalfExtents = Vector3.one * 0.5f;

        [SerializeField] private LayerMask m_LayerMask = Physics.DefaultRaycastLayers;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"check Box 3D at {this.m_Position}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            return Physics.CheckBox(
                this.m_Position.Get(args),
                this.m_HalfExtents,
                this.m_Rotation.Get(args),
                this.m_LayerMask
            );
        }
    }
}
