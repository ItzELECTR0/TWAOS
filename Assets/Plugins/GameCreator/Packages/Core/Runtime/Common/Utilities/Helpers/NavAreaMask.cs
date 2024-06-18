using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class NavAreaMask
    {
        [SerializeField] private int m_Mask = -1;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public int Mask => this.m_Mask;
        
        // OVERRIDES: -----------------------------------------------------------------------------

        public override string ToString()
        {
            #if UNITY_EDITOR
            
            switch (this.m_Mask)
            {
                case 0: return "Nothing";
                case -1: return "Everything";
            }

            string[] areaNames = UnityEngine.AI.NavMesh.GetAreaNames();

            for (int i = 0; i < 32; ++i)
            {
                int index = 1 << i;
                
                if ((index & this.m_Mask) == 0) continue;
                string name = areaNames[i];
        
                return (~index & this.m_Mask) == 0 ? name : "(mixed)";
            }

            #endif

            return "(unknown)";
        }
    }
}