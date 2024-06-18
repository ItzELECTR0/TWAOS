using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public static partial class CycleTargets
    {
        public static void Closest(Character character)
        {
            Targets targets = character.Combat.Targets;
            List<GameObject> list = targets.List;

            float minDistance = INFINITY;
            GameObject nextCandidate = null;

            foreach (GameObject candidate in list)
            {
                Vector3 position = candidate.transform.position;
                float distance = Vector3.Distance(character.transform.position, position);
                
                if (distance >= minDistance) continue;
                
                minDistance = distance;
                nextCandidate = candidate;
            }
            
            targets.Primary = nextCandidate;
        }
    }
}