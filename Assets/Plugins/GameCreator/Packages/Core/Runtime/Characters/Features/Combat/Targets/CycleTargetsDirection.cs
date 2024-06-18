using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public static partial class CycleTargets
    {
        public static void Direction(Character character, Camera camera, Vector2 direction)
        {
            if (camera == null) return;
            if (direction.sqrMagnitude <= 0f) return;

            Targets targets = character.Combat.Targets;
            List<GameObject> list = targets.List;

            Vector3 origin = camera.transform.TransformPoint(Vector3.forward);
            Vector3 originPoint = camera.WorldToScreenPoint(origin);

            float minAngle = INFINITY;
            GameObject nextCandidate = null;

            foreach (GameObject candidate in list)
            {
                if (candidate == targets.Primary) continue;
                
                Vector3 point = camera.WorldToScreenPoint(candidate.transform.position);
                float angle = Vector2.Angle(point - originPoint, direction);

                if (angle >= Math.Min(minAngle, 90f)) continue;
                
                minAngle = angle;
                nextCandidate = candidate;
            }

            if (nextCandidate != null)
            {
                targets.Primary = nextCandidate;
            }
        }
    }
}