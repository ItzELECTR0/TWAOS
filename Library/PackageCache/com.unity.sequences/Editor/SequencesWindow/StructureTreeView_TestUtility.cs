#if UNITY_INCLUDE_TESTS
using System.Linq;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    internal partial class StructureTreeView
    {
        internal bool CanDeleteSelection_Test()
        {
            return CanDeleteSelection();
        }

        internal bool CanRename_Test(int index)
        {
            return CanRename(index);
        }

        internal void DeleteItem(TimelineAsset timeline)
        {
            ExpandAll(); // Ensures that we can find the corresponding index.

            var id = timeline.GetHashCode();
            var index = viewController.GetIndexForId(id);
            SetSelection(index);
            DeleteSelectedItems();
        }

        internal bool HasItem(TimelineAsset timeline)
        {
            var id = timeline.GetHashCode();
            return viewController.GetAllItemIds().Contains(id);
        }

        internal VisualElement GetVisualElementFromData(TimelineAsset timeline)
        {
            var id = timeline.GetHashCode();

            ExpandAll();

            var rootElement = GetRootElementForId(id);
            return rootElement.Q<VisualElement>(itemClassName);
        }

        internal void BeginRenameItem(TimelineAsset timeline)
        {
            var id = timeline.GetHashCode();
            ExpandAll();

            var index = viewController.GetIndexForId(id);
            BeginRenameAtIndex(index, 0);
        }

        internal TimelineAsset GetSelectedItemTimeline()
        {
            var item = selectedItem as SequenceNode;
            return item.timeline;
        }
    }
}
#endif // UNITY_INCLUDE_TESTS
