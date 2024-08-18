using UnityEditor;
using UnityEngine.AI;

namespace GameCreator.Editor.Common
{
    public abstract class TNavAreaDrawer : PropertyDrawer
    {
        // PROTECTED METHODS: ---------------------------------------------------------------------
        
        protected static string IndexToName(int index)
        {
            string[] areaNames = NavMesh.GetAreaNames();
            foreach (string areaName in areaNames)
            {
                int areaIndex = NavMesh.GetAreaFromName(areaName);
                if (areaIndex == index) return areaName;
            }

            return string.Empty;
        }

        protected static string[] Areas()
        {
            string[] areas =
            {
                "", "", "", "", "", "", "", "",
                "", "", "", "", "", "", "", "",
                "", "", "", "", "", "", "", "",
                "", "", "", "", "", "", "", ""
            };
            
            string[] areaNames = NavMesh.GetAreaNames();
            foreach (string areaName in areaNames)
            {
                int index = NavMesh.GetAreaFromName(areaName);
                areas[index] = areaName;
            }

            return areas;
        }
    }
}