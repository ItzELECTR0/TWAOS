using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public class Phase
    {
        private struct Layer
        {
            public float value;
            public float weight;
        }
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private readonly List<Layer> m_Layers = new List<Layer>();

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public float Get(float source)
        {
            float value = source;
            
            for (int i = this.m_Layers.Count - 1; i >= 0; --i)
            {
                Layer layer = this.m_Layers[i];
                value = Mathf.Lerp(value, layer.value, layer.weight);
            }

            return value;
        }
        
        public void Add(float value, float weight)
        {
            this.m_Layers.Add(new Layer
            {
                value = value,
                weight = weight
            });
        }

        public void Reset()
        {
            this.m_Layers.Clear();
        }
    }
}