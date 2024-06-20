using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Sequences;
using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    [PackageHelpURL("sequences-window")]
    internal class SequencesWindow : TreeViewEditorWindow<StructureTreeView>
    {
        internal const string k_CreateMasterSequenceMenuActionName = "Create Master Sequence";
        internal const string k_CreateSequenceMenuActionName = "Create Sequence";

        protected override void SetupView()
        {
            base.SetupView();

            titleContent = new GUIContent(
                "Sequences",
                IconUtility.LoadIcon("MasterSequence/MasterSequence", IconUtility.IconType.UniqueToSkin));

            AddManipulator(new ContextualMenuManipulator(OnContextMenuClick));

#if UNITY_2022_2_OR_NEWER
            var searchField = new DebouncedToolbarSearchField();
#else
            var searchField = new ToolbarSearchField();
#endif
            searchField.RegisterValueChangedCallback(evt => SendSearchEvent(evt.newValue));
            SetHeaderContent(searchField);

            // Clear search on escape and intercept the event that deselects tree view items.
            treeView.scrollViewContainer.RegisterCallback<NavigationCancelEvent>(evt =>
            {
                var isSearching = !string.IsNullOrEmpty(searchField.value);

                if (isSearching)
                {
#if !UNITY_2023_2_OR_NEWER
                    evt.PreventDefault(); // Obsolete in 2023.2+
#endif
                    evt.StopImmediatePropagation();
                    searchField.value = string.Empty;
                }
            }, TrickleDown.TrickleDown);
        }

        protected override string GetAddMenuTooltip()
        {
            return "Create a new Sequence or Master Sequence";
        }

        protected override void PopulateAddMenu(DropdownMenu menu, bool contextual = false)
        {
            menu.AppendAction(k_CreateMasterSequenceMenuActionName, CreateMasterSequenceAction);

            if (!contextual)
                menu.AppendAction(k_CreateSequenceMenuActionName, CreateSequenceAction, CreateSequenceActionStatus);
        }

        void CreateMasterSequenceAction(DropdownMenuAction action)
        {
            BeginSequenceCreation();
        }

        void CreateSequenceAction(DropdownMenuAction action)
        {
            BeginSequenceCreation(treeView.selectedIndex);
        }

        DropdownMenuAction.Status CreateSequenceActionStatus(DropdownMenuAction action)
        {
            if (treeView.selectedIndex == -1 || treeView.selectedIndices.Count() > 1)
                return DropdownMenuAction.Status.Disabled;

            return treeView.GetCreateSequenceActionStatus(treeView.selectedIndex);
        }

        void BeginSequenceCreation(int parentIndex = -1)
        {
            var parentId = treeView.GetIdForIndex(parentIndex);
            treeView.BeginItemCreation(parentId);
        }

        void OnContextMenuClick(ContextualMenuPopulateEvent evt)
        {
            PopulateAddMenu(evt.menu, true);
        }

        void SendSearchEvent(string query)
        {
            using var searchEvent = SearchEvent.GetPooled(query);
            searchEvent.target = treeView;
            treeView.SendEvent(searchEvent);
        }
    }
}
