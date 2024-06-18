using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Isometric 8 Directions")]
    [Category("Isometric/Isometric 8 Directions")]

    [Image(typeof(IconIsometric), ColorTheme.Type.Yellow)]
    [Description("Snaps the character direction in multiples of 45 degrees")]

    [Serializable]
    public class AxonometryIsometric8Directions : TAxonometry
    {
        private static readonly Vector3[] ISOMETRIC_DIRECTIONS = 
        {
            new Vector2( 1f,  0f), // E
            new Vector2( 1f,  1f), // NE
            new Vector2( 0f,  1f), // N
            new Vector2(-1f,  1f), // NW
            new Vector2(-1f,  0f), // W
            new Vector2(-1f, -1f), // SW
            new Vector2( 0f, -1f), // S
            new Vector2( 1f, -1f), // SE
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
            int octant = (Mathf.RoundToInt(8f * radians / (Mathf.PI * 2f)) + 8) % 8;

            return ISOMETRIC_DIRECTIONS[octant].normalized * magnitude;
        }
        
        // CLONE: ---------------------------------------------------------------------------------

        public override object Clone()
        {
            return new AxonometryIsometric8Directions();
        }
        
        // STRING: --------------------------------------------------------------------------------
        
        public override string ToString() => "8 Directions";
    }
}