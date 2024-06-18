using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public static partial class SkeletonBuilder
    {
        private static void CalculateDirection(Vector3 point, out int direction, out float distance)
        {
            direction = 0;
            if (Mathf.Abs(point[1]) > Mathf.Abs(point[0])) direction = 1;
            if (Mathf.Abs(point[2]) > Mathf.Abs(point[direction])) direction = 2;

            distance = point[direction];
        }
    }
}