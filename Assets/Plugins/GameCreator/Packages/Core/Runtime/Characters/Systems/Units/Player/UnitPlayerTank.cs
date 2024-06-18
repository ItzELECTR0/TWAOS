using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Title("Tank")]
    [Image(typeof(IconTank), ColorTheme.Type.Green)]
    
    [Category("Tank")]
    [Description(
        "Moves the Player using a directional input from the Player's perspective"
    )]

    [Serializable]
    public class UnitPlayerTank : UnitPlayerDirectional
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override Type ForceFacing => typeof(UnitFacingTank);
        
        // METHODS: -------------------------------------------------------------------------------
        
        protected override Vector3 GetMoveDirection(Vector3 input)
        {
            Vector3 direction = new Vector3(0f, 0f, input.y);
            Vector3 moveDirection = this.Transform.TransformDirection(direction);

            moveDirection.Scale(Vector3Plane.NormalUp);
            moveDirection.Normalize();

            return moveDirection * direction.magnitude;
        }

        // STRING: --------------------------------------------------------------------------------

        public override string ToString() => "Tank";
    }
}