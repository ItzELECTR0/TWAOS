using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class LayerMaskValue
    {
        [SerializeField] private int m_Value = 0;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public int Value => this.m_Value;
        
        // OVERRIDES: -----------------------------------------------------------------------------

        public override string ToString()
        {
            if (this.m_Value < 0 || this.m_Value > 31) return "(unknown)"; 
            string value = LayerMask.LayerToName(this.m_Value);
            return !string.IsNullOrEmpty(value) ? value : "(unnamed)";
        }
        
        // STATIC METHODS: ------------------------------------------------------------------------

        public static string GetLayerMaskName(LayerMask mask)
        {
            int bitmask = mask.value;
            
            if (bitmask == 0) return "Nothing";
            if (bitmask == -1) return "Everything";

            for (int i = 0; i < 32; ++i)
            {
                int value = 1 << i;
                
                if ((value & bitmask) == 0) continue;
                string name = LayerMask.LayerToName(i);

                return (~value & bitmask) == 0 ? name : "(mixed)";
            }

            return "(unknown)";
        }
    }
}