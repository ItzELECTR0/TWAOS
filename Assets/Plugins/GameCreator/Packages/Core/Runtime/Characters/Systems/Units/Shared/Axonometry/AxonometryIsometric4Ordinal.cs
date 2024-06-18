using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Isometric Ordinal")]
    [Category("Isometric/Isometric Ordinal")]

    [Image(typeof(IconIsometric), ColorTheme.Type.Yellow, typeof(OverlayCross))]
    [Description("Snaps the character direction in 45 degree diagonals in world space")]

    [Serializable]
    public class AxonometryIsometric4Ordinal : TAxonometry
    {
        private static readonly Vector3[] ISOMETRIC_DIRECTIONS = 
        {
            new Vector2( 1f,  1f), // NE
            new Vector2(-1f,  1f), // NW
            new Vector2(-1f, -1f), // SW
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
            int quadrant = (Mathf.RoundToInt(4f * radians / (Mathf.PI * 2f)) + 4) % 4;

            return ISOMETRIC_DIRECTIONS[quadrant].normalized * magnitude;
        }
        
        // CLONE: ---------------------------------------------------------------------------------

        public override object Clone()
        {
            return new AxonometryIsometric4Ordinal();
        }
        
        // STRING: --------------------------------------------------------------------------------
        
        public override string ToString() => "4 Ordinal";
    }
}