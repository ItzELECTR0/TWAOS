using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Isometric Cardinal")]
    [Category("Isometric/Isometric Cardinal")]

    [Image(typeof(IconIsometric), ColorTheme.Type.Yellow, typeof(OverlayPlus))]
    [Description("Snaps the character direction in either up, down, right or left")]

    [Serializable]
    public class AxonometryIsometric4Cardinal : TAxonometry
    {
        private static readonly Vector3[] ISOMETRIC_DIRECTIONS = 
        {
            new Vector2( 1f,  0f), // E
            new Vector2( 0f,  1f), // N
            new Vector2(-1f,  0f), // W
            new Vector2( 0f, -1f), // S
        };
        
        public override Vector3 ProcessTranslation(TUnitDriver driver, Vector3 movement)
        {
            Vector2 newDirection = IsometricDirection(movement.XZ());
            return new Vector3(newDirection.x, movement.y, newDirection.y);
        }

        public override Vector3 ProcessRotation(TUnitFacing facing, Vector3 direction)
        {
            Vector2 newDirection = IsometricDirection(direction.XZ());
            return new Vector3(newDirection.x, direction.y, newDirection.y);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private Vector2 IsometricDirection(Vector2 direction)
        {
            float magnitude = direction.magnitude;
            
            if (magnitude <= float.Epsilon) return direction;

            float radians = Mathf.Atan2(direction.y, direction.x);
            int quadrant = (Mathf.RoundToInt(4f * radians / (Mathf.PI * 2f)) + 4) % 4;

            return ISOMETRIC_DIRECTIONS[quadrant].normalized * magnitude;
        }
        
        // CLONE: ---------------------------------------------------------------------------------

        public override object Clone()
        {
            return new AxonometryIsometric4Cardinal();
        }
        
        // STRING: --------------------------------------------------------------------------------
        
        public override string ToString() => "4 Cardinal";
    }
}