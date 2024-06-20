using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Sequences;

namespace UnityEditor.Sequences.Startup
{
    [InitializeOnLoad]
    class SetupMasterSequenceDragAndDrop
    {
        static SetupMasterSequenceDragAndDrop()
        {
            DragAndDrop.AddDropHandler(HierarchyDropHandler);
            MasterSequence.sequenceAdded += RefreshTimelineEditor;
            MasterSequence.sequenceRemoved += RefreshTimelineEditor;
        }

        static DragAndDropVisualMode HierarchyDropHandler(int dropTargetInstanceID,
            HierarchyDropFlags dropFlags, Transform parentForDraggedObjects, bool perform)
        {
            MasterSequence masterSequence = null;
            foreach (var objectReference in DragAndDrop.objectReferences)
            {
                if (objectReference is MasterSequence)
                {
                    masterSequence = objectReference as MasterSequence;
                    break;
                }
            }

            if (masterSequence == null)
                return DragAndDropVisualMode.None;

            if (perform)
            {
                var sequenceFilters = ObjectsCache.FindObjectsFromScenes<SequenceFilter>();
                foreach (var sequenceFilter in sequenceFilters)
                {
                    if (sequenceFilter.masterSequence == masterSequence)
                    {
                        Debug.Log("MasterSequence \"" + masterSequence.name + "\" already exists in the scene.");
                        return DragAndDropVisualMode.Generic;
                    }
                }
                // parentForDraggedObjects is always null in this call. We expect it to vary when the cinemactic is dragged and dropped under
                // different objects in the hierarchy. It will remain for now, but make sure to double-check it when te API releases.
                SequenceFilter.GenerateSequenceRepresentation(masterSequence, masterSequence.rootSequence, parentForDraggedObjects);

                if (EditorWindow.HasOpenInstances<SequencesWindow>())
                    EditorWindow.GetWindow<SequencesWindow>().treeView.RefreshItems();
            }

            return DragAndDropVisualMode.Generic;
        }

        static void RefreshTimelineEditor(MasterSequence masterSequence, Sequence sequence)
        {
            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
        }
    }
}
