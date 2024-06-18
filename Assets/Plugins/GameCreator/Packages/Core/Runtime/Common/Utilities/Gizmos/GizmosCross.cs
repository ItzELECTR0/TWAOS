using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public static partial class GizmosExtension
    {
        public enum CrossDirection
        {
            Forward,
            Upwards,
            Sidewards
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static void Cross(Vector3 position, CrossDirection direction, float radius)
        {
            Vector3 pointA1, pointA2;
            Vector3 pointB1, pointB2;

            switch (direction)
            {
                case CrossDirection.Forward:
                    pointA1 = position + Vector3.up * radius;
                    pointA2 = position - Vector3.up * radius;
                    pointB1 = position + Vector3.right * radius;
                    pointB2 = position - Vector3.right * radius;
                    break;
                
                case CrossDirection.Upwards:
                    pointA1 = position + Vector3.forward * radius;
                    pointA2 = position - Vector3.forward * radius;
                    pointB1 = position + Vector3.right * radius;
                    pointB2 = position - Vector3.right * radius;
                    break;
                
                case CrossDirection.Sidewards:
                    pointA1 = position + Vector3.forward * radius;
                    pointA2 = position - Vector3.forward * radius;
                    pointB1 = position + Vector3.up * radius;
                    pointB2 = position - Vector3.up * radius;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
            
            Gizmos.DrawLine(pointA1, pointA2);
            Gizmos.DrawLine(pointB1, pointB2);
        }
    }
}