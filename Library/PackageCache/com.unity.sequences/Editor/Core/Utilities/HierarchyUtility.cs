using UnityEngine;

namespace UnityEditor.Sequences
{
    /// <summary>
    /// Utility class for anything related to the Hierarchy.
    /// </summary>
    internal static class HierarchyUtility
    {
        public static T GetFirstParentOfType<T>(GameObject gameObject) where T : Component
        {
            var parents = gameObject.GetComponentsInParent<T>(includeInactive: true);
            if (parents == null || parents.Length == 0)
                return null;

            return parents[0];
        }
    }
}
