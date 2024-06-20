using System;
using UnityEngine.Sequences;
using PrefabStage = UnityEditor.SceneManagement.PrefabStage;

namespace UnityEditor.Sequences
{
    /// <summary>
    /// Class in charge of reflecting changes from the Hierarchy window
    /// to the data.
    /// </summary>
    [InitializeOnLoad]
    class HierarchyChangeListener
    {
        internal static event Action sequencePrefabInstantiated; // Only used in tests.

        static HierarchyChangeListener()
        {
            EditorApplication.hierarchyChanged += UpdateMasterSequence;
            PrefabStage.prefabStageOpened += SequencePrefabOpened;
        }

        static void UpdateMasterSequence()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            var sequenceFilters = ObjectsCache.FindObjectsFromScenes<SequenceFilter>();
            foreach (var sequenceFilter in sequenceFilters)
            {
                if (sequenceFilter.masterSequence == null)
                    continue;

                // Process instantiation of "prefabized" sequences.
                var parent = sequenceFilter.transform.parent;
                if ((parent == null || parent.GetComponent<SequenceFilter>() == null) && PrefabUtility.IsOutermostPrefabInstanceRoot(sequenceFilter.gameObject))
                {
                    sequenceFilter.gameObject.SetActive(true);
                    sequencePrefabInstantiated?.Invoke();
                }
            }
        }

        static void SequencePrefabOpened(PrefabStage stage)
        {
            var root = stage.prefabContentsRoot;
            if (root.GetComponent<SequenceFilter>() != null)
                root.SetActive(true);
        }
    }
}
