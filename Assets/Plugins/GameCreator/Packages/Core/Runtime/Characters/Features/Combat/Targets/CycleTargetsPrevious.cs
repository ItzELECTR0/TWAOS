using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public static partial class CycleTargets
    {
        public static void Previous(Character character)
        {
            Targets targets = character.Combat.Targets;
            List<GameObject> list = targets.List;
            
            if (targets.Primary == null)
            {
                if (list.Count == 0) return;
                targets.Primary = list[0];
            }
            
            if (list.Count == 0) return;
            int primaryIndex = list.IndexOf(targets.Primary);
            
            if (primaryIndex < 0)
            {
                targets.Primary = list[0];
                return;
            }

            int prevIndex = --primaryIndex < 0 ? list.Count - 1 : primaryIndex;
            targets.Primary = list[prevIndex];
        }
    }
}