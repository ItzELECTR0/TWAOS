using System.IO;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Sequences;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    partial class StructureTreeView
    {
        void PopulateContextMenu(DropdownMenu menu)
        {
            if (selectedIndices.Count() > 1)
                MultiSelectionContextMenu(menu);

            else
            {
                var itemData = GetItemDataForIndex(selectedIndex);
                if (itemData.timeline == null)
                {
                    InvalidSequenceContextMenu(menu, selectedIndex);
                    return;
                }

                var sequence = SequenceIndexer.instance.GetSequence(itemData.timeline);
                if (!sequence.isValid)
                {
                    InvalidSequenceContextMenu(menu, selectedIndex);
                    return;
                }

                var masterSequence = sequence.masterSequence;
                var editorialLoadedStatus = GetEditorialLoadedStatus(masterSequence);
                if (editorialLoadedStatus != EditorialLoadedStatus.None)
                    RegularSequenceContextMenu(menu, selectedIndex);
                else
                    UnloadedSequenceContextMenu(menu, masterSequence.timeline);
            }
        }

        void MultiSelectionContextMenu(DropdownMenu menu)
        {
            menu.AppendAction("Delete selected", DeleteItemsAction, DeleteItemsActionStatus, selectedIndices.ToArray());
        }

        void RegularSequenceContextMenu(DropdownMenu menu, int index)
        {
            if (menu.MenuItems().Any())
                return;

            menu.AppendAction("Create Sequence", CreateSequenceAction, CreateSequenceActionStatus, index);

            // Sequence basic operation
            menu.AppendSeparator();
            menu.AppendAction("Rename", RenameItemAction, RenameItemActionStatus, index);
            // TODO: Duplicate action would go here.
            menu.AppendAction("Delete", DeleteItemsAction, DeleteItemsActionStatus, new[] {index});

            // Scene management
            AppendSceneManagementActions(menu, index);

            // Recorder
            menu.AppendSeparator();
            menu.AppendAction("Record...", RecordItemAction, RecordItemActionStatus, index);
        }

        void UnloadedSequenceContextMenu(DropdownMenu menu, TimelineAsset masterTimeline)
        {
            menu.AppendAction("Open Master Scene", LoadMasterSceneAction, LoadMasterSceneActionStatus, masterTimeline);
            menu.AppendAction("Instantiate in active Scene", InstantiateInHierarchyAction, InstantiateInHierarchyActionStatus, masterTimeline);
        }

        void InvalidSequenceContextMenu(DropdownMenu menu, int index)
        {
            menu.AppendAction("Delete", DeleteItemsAction, DeleteItemsActionStatus, new[] {index});
        }

        void CreateSequenceAction(DropdownMenuAction action)
        {
            var index = (int)action.userData;
            var id = GetIdForIndex(index);
            BeginItemCreation(id);
        }

        DropdownMenuAction.Status CreateSequenceActionStatus(DropdownMenuAction action)
        {
            return GetCreateSequenceActionStatus((int)action.userData);
        }

        // This function is internal because it is also used by the Sequences window to check
        // the status for the top "+" menu.
        internal DropdownMenuAction.Status GetCreateSequenceActionStatus(int index)
        {
            if (inPlaymode)
                return DropdownMenuAction.Status.Disabled;

            var itemData = GetItemDataForIndex(index);
            var sequence = SequenceIndexer.instance.GetSequence(itemData.timeline);

            if (sequence.hasGameObject)
                return DropdownMenuAction.Status.Normal;

            return DropdownMenuAction.Status.Disabled;
        }

        void RenameItemAction(DropdownMenuAction action)
        {
            var index = (int)action.userData;
            BeginRenameAtIndex(index);
        }

        DropdownMenuAction.Status RenameItemActionStatus(DropdownMenuAction action)
        {
            var index = (int)action.userData;
            if (CanRename(index))
                return DropdownMenuAction.Status.Normal;

            return DropdownMenuAction.Status.Disabled;
        }

        void DeleteItemsAction(DropdownMenuAction action)
        {
            DeleteSelectedItemsInternal();
        }

        DropdownMenuAction.Status DeleteItemsActionStatus(DropdownMenuAction action)
        {
            if (CanDeleteSelection())
                return DropdownMenuAction.Status.Normal;

            return DropdownMenuAction.Status.Disabled;
        }

        void RecordItemAction(DropdownMenuAction action)
        {
            var index = (int)action.userData;
            var itemData = GetItemDataForIndex(index);
            MasterSequenceUtility.GetLegacyData(itemData.timeline, out _, out var sequence);
            sequence.Record();
        }

        DropdownMenuAction.Status RecordItemActionStatus(DropdownMenuAction action)
        {
            if (inPlaymode || inPrefabStage)
                return DropdownMenuAction.Status.Disabled;

            var index = (int)action.userData;
            var itemData = GetItemDataForIndex(index);

            MasterSequenceUtility.GetLegacyData(itemData.timeline, out _, out var sequenceLegacy);
            sequenceLegacy.GetRecordFrameStartAndEnd(out var frameStart, out var frameEnd);
            if (frameStart == 0 && frameEnd == 0)
            {
                // Sequence to record is not 'accessible'.
                return DropdownMenuAction.Status.Disabled;
            }

            var sequence = SequenceIndexer.instance.GetSequence(itemData.timeline);
            if (sequence.hasGameObject)
                return DropdownMenuAction.Status.Normal;

            return DropdownMenuAction.Status.Disabled;
        }

        void LoadMasterSceneAction(DropdownMenuAction action)
        {
            var masterTimeline = action.userData as TimelineAsset;
            var path = MasterSequenceRegistryUtility.GetMasterScene(masterTimeline);

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(path);
                RefreshItems();
            }
        }

        DropdownMenuAction.Status LoadMasterSceneActionStatus(DropdownMenuAction action)
        {
            if (inPlaymode)
                return DropdownMenuAction.Status.Disabled;

            var masterTimeline = action.userData as TimelineAsset;
            var path = MasterSequenceRegistryUtility.GetMasterScene(masterTimeline);

            if (string.IsNullOrEmpty(path) || SceneManagement.IsLoaded(path))
                return DropdownMenuAction.Status.Disabled;

            return DropdownMenuAction.Status.Normal;
        }

        void InstantiateInHierarchyAction(DropdownMenuAction action)
        {
            var masterTimeline = action.userData as TimelineAsset;
            var scenePath = MasterSequenceRegistryUtility.GetMasterScene(masterTimeline);
            var activeScene = SceneManager.GetActiveScene();

            MasterSequenceUtility.GetLegacyData(masterTimeline, out var masterSequence, out _);
            SequenceFilter.GenerateSequenceRepresentation(masterSequence, masterSequence.rootSequence, null);
            SelectionUtility.TrySelectSequenceWithoutNotify(masterTimeline);
            EditorSceneManager.MarkSceneDirty(activeScene);
            RefreshItems();

            if (scenePath != string.Empty && scenePath != activeScene.path)
            {
                var registry = MasterSequenceRegistryUtility.GetRegistry(masterTimeline);
                var registryPath = AssetDatabase.GetAssetPath(registry);
                var sceneName = Path.GetFileNameWithoutExtension(scenePath);

                Debug.LogWarning($"Editorial structure \"{masterTimeline.name}\" has a different Master Scene " +
                    $"(\"{sceneName}\") than the current active scene. Having the structure in multiple scenes may " +
                    "affect the result of create, rename and delete operations on its sequences.\n\n" +
                    $"You can change the Master Scene associated to \"{masterTimeline.name}\" in its " +
                    $"MasterSequenceRegistry asset at: {registryPath}.\n");
            }
        }

        DropdownMenuAction.Status InstantiateInHierarchyActionStatus(DropdownMenuAction action)
        {
            if (inPlaymode)
                return DropdownMenuAction.Status.Disabled;

            return DropdownMenuAction.Status.Normal;
        }

        bool inPlaymode => EditorApplication.isPlayingOrWillChangePlaymode;
        bool inPrefabStage => PrefabStageUtility.GetCurrentPrefabStage() != null;

        enum EditorialLoadedStatus
        {
            None,
            Fully,
            Partially
        }

        EditorialLoadedStatus GetEditorialLoadedStatus(SequenceNode sequence)
        {
            var loadedStatus = sequence.isValid && sequence.hasGameObject ? EditorialLoadedStatus.Fully : EditorialLoadedStatus.None;
            foreach (var child in sequence.children)
            {
                var currentLoadedStatus = GetEditorialLoadedStatus(child);
                if (loadedStatus != currentLoadedStatus)
                    return EditorialLoadedStatus.Partially;
            }

            return loadedStatus;
        }
    }
}
