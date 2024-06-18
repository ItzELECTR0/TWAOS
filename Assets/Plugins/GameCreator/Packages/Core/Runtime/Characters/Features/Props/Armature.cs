using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    internal class Armature : Dictionary<string, Transform>
    {
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public Armature(Character character, Transform transform)
        {
            GatherBones(character, transform);
        }
        
        public Transform Get(string name)
        {
            return this.ContainsKey(name) ? this[name] : null;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void GatherBones(Character character, Transform transform)
        {
            if (character != null && character.Props.HasInstance(transform.gameObject)) return;
            
            if (this.ContainsKey(transform.name))
            {
                Remove(transform.name);
            }

            Add(transform.name, transform);
            int childCount = transform.childCount;
                
            for (int i = 0; i < childCount; ++i)
            {
                this.GatherBones(character, transform.GetChild(i));
            }
        }
    }
}