using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    partial class StructureTreeView : SequencesTreeView
    {
        public static readonly string masterSequenceIconClassName = itemIconClassName + "-master-sequence";
        public static readonly string sequenceIconClassName = itemIconClassName + "-sequence";
        public static readonly string shotIconClassName = itemIconClassName + "-shot";
        public static readonly string itemInvalidClassName = itemLabelClassName + "-invalid";
        public static readonly string itemNotLoadedClassName = itemLabelClassName + "-not-loaded";

        bool m_PreventSelectionLoop = false;
        string m_SearchQuery;
        IComparer<SequenceNode> m_Comparer = new SequenceNodeNameComparer();

        internal class SequenceNodeNameComparer : IComparer<SequenceNode>
        {
            public int Compare(SequenceNode x, SequenceNode y)
            {
                if (x == null) return -1;
                if (y == null) return 1;

                return x.GetDisplayName().CompareTo(y.GetDisplayName());
            }
        }

        public StructureTreeView() : base()
        {
            if (SequenceIndexer.instance.isEmpty)
            {
                // If the indexer is not yet initialized, delay the tree view data generation.
                SequenceIndexer.indexerInitialized += InitializeRootItems;
                SetRootItems(new List<TreeViewItemData<SequenceNode>>());
                return;
            }

            SequenceIndexer.indexerInitialized -= InitializeRootItems;
            InitializeRootItems();
        }

        protected override string GetItemTextForIndex(int index)
        {
            var itemData = GetItemDataForIndex<SequenceNode>(index);
            if (itemData != default)
                return itemData.GetDisplayName();

            var parentId = GetParentIdForIndex(index);
            return parentId == -1
                ? SequenceUtility.k_DefaultMasterSequenceName
                : SequenceUtility.k_DefaultSequenceName;
        }

        protected override void ContextClicked(DropdownMenu menu)
        {
            PopulateContextMenu(menu);
        }

        protected override void DeleteSelectedItems()
        {
            if (CanDeleteSelection())
                DeleteSelectedItemsInternal();
        }

        void DeleteSelectedItemsInternal()
        {
            var timelinesToDelete = new List<TimelineAsset>();
            var items = GetSelectedItems<SequenceNode>().ToArray();

            foreach (var item in items)
            {
                if (item.data.timeline != null)
                    timelinesToDelete.Add(item.data.timeline);
            }

            if (timelinesToDelete.Count > 0 && !UserVerifications.ValidateSequencesDeletion(timelinesToDelete.ToArray()))
                return;

            foreach (var item in items)
            {
                if (item.data.timeline == null)
                {
                    DeleteInvalidItem(viewController.GetIndexForId(item.id));
                    viewController.TryRemoveItem(item.id, false);
                    continue;
                }

                MasterSequenceUtility.GetLegacyData(item.data.timeline, out var masterSequence, out var sequence);
                if (masterSequence.rootSequence == sequence)
                {
                    using (new SequenceIndexer.DisableEvent())
                        masterSequence.Delete();
                }
                else
                {
                    using (new SequenceIndexer.DisableEvent())
                        SequenceUtility.DeleteSequence(sequence, masterSequence);
                }
            }

            RebuildTree();
            RefreshItems();
        }

        void DeleteInvalidItem(int index)
        {
            var parentId = GetParentIdForIndex(index);
            if (parentId != -1 && IsSelected(viewController.GetIndexForId(parentId)))
                return;

            var itemData = GetItemDataForIndex(index);
            if (itemData == null)
                return;

            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Delete invalid sequence");
            var groupIndex = Undo.GetCurrentGroup();

            if (itemData.gameObject != null)
                Undo.DestroyObjectImmediate(itemData.gameObject);

            if (parentId == -1)
            {
                Undo.CollapseUndoOperations(groupIndex);
                return;
            }

            var parentData = GetItemDataForId(parentId);
            if (parentData.timeline != null)
            {
                var parentSequence = SequenceIndexer.instance.GetSequence(parentData.timeline);

                Undo.RecordObject(parentSequence.timeline, "Delete invalid sequence");
                parentSequence.timeline.DeleteClip(itemData.editorialClip);
            }

            Undo.CollapseUndoOperations(groupIndex);
            // Ideally, UI should refresh on undo/redo of this.
        }

        protected override void RenameEnded(int id, bool canceled = false)
        {
            var itemData = GetItemDataForId(id);

            var root = GetRootElementForId(id);
            var label = root.Q<RenameableLabel>();
            var newName = label.text;

            canceled |= itemData != null && string.IsNullOrWhiteSpace(newName);
            if (canceled)
            {
                if (itemData == null)
                    TryRemoveItem(id);

                var index = viewController.GetIndexForId(id);
                RefreshItem(index);
                return;
            }

            newName = FilePathUtility.SanitizeFileName(newName);
            label.text = newName;

            if (itemData == null)
            {
                if (string.IsNullOrWhiteSpace(newName))
                {
                    // TODO: This validation (and more) should be dealt with when actually creating or renaming a sequence.
                    var index = viewController.GetIndexForId(id);
                    newName = GetItemTextForIndex(index);
                }

                // Create a new MasterSequence or Sequence.
                var parentId = viewController.GetParentId(id);
                viewController.TryRemoveItem(id, false); // Remove the dummy item. Don't rebuild the tree, it will be rebuilt when creating a definitive item.

                if (parentId == -1)
                {
                    SequenceUtility.CreateMasterSequence(newName);
                }
                else
                {
                    var parentItemData = GetItemDataForId(parentId);
                    if (parentItemData.timeline == null)
                        return;

                    MasterSequenceUtility.GetLegacyData(parentItemData.timeline, out var masterSequence,
                        out var parentSequence);
                    SequenceUtility.CreateSequence(
                        newName,
                        masterSequence,
                        parentSequence);
                }
            }
            else
            {
                // Rename
                MasterSequenceUtility.GetLegacyData(itemData.timeline, out var masterSequence, out var timelineSequence);
                if (itemData.parent != null)
                {
                    timelineSequence.Rename(newName);
                }
                else
                {
                    masterSequence.Rename(newName);
                }
            }
        }

        protected override void InitClassListAtIndex(VisualElement ve, int index)
        {
            var itemData = GetItemDataForIndex(index);
            var isItemBeingCreated = itemData == null;

            var itemDepth = isItemBeingCreated
                ? GetDepthOfItemBeingCreated(index)
                : GetDepthOfItem(itemData);

            var iconClassName = itemDepth switch
            {
                0 => masterSequenceIconClassName,
                1 => sequenceIconClassName,
                _ => shotIconClassName,
            };

            var icon = GetIconElement(ve);
            icon.EnableInClassList(iconClassName, true);

            // Check validity/loading state of data
            var label = GetLabelElement(ve);

            if (itemData == default)
            {
                label.EnableInClassList(itemInvalidClassName, false);
                label.EnableInClassList(itemNotLoadedClassName, false);
                return;
            }

            if (itemData.timeline == null)
                label.EnableInClassList(itemInvalidClassName, true);

            else
            {
                var sequence = SequenceIndexer.instance.GetSequence(itemData.timeline);

                if (!sequence.isValid) // Validity is more important than loading status.
                    label.EnableInClassList(itemInvalidClassName, true);

                else if (!sequence.hasGameObject)
                    label.EnableInClassList(itemNotLoadedClassName, true);
            }
        }

        protected override void ResetClassListAtIndex(VisualElement ve, int index)
        {
            var icon = GetIconElement(ve);
            icon.EnableInClassList(masterSequenceIconClassName, false);
            icon.EnableInClassList(sequenceIconClassName, false);
            icon.EnableInClassList(shotIconClassName, false);

            var label = GetLabelElement(ve);
            label.EnableInClassList(itemInvalidClassName, false);
            label.EnableInClassList(itemNotLoadedClassName, false);
        }

        protected override string GetTooltipForIndex(int index)
        {
            var itemData = GetItemDataForIndex<SequenceNode>(index);
            if (itemData == default)
                return string.Empty;

            var tooltips = string.Empty;

            SequenceNode sequence = null;
            if (itemData.timeline != null)
                sequence = SequenceIndexer.instance.GetSequence(itemData.timeline);

            if (sequence != null && sequence.isValid && !sequence.hasGameObject)
                tooltips = "Not in the Hierarchy";

            else if (sequence != null && sequence.parent != null && !sequence.parent.isValid)
                tooltips = "Invalid parent Sequence";

            else if (itemData.timeline == null)
                tooltips = "Missing Timeline asset or missing binding on the PlayableDirector";

            else if (sequence != null && !sequence.isValid && sequence.gameObject == null ||
                     sequence == null && itemData.gameObject == null)
            {
                tooltips = "Missing GameObject or PlayableDirector";
            }
            else if (sequence != null && !sequence.isValid)
                tooltips = "Missing binding on the Editorial clip";

            return tooltips;
        }

        protected override bool CanRename(int index)
        {
            if (inPlaymode)
                return false;

            var itemData = GetItemDataForIndex(index);
            if (itemData == default)
                return true; // Item is being created, it can be renamed.

            if (itemData.timeline == null)
                return false;

            var sequence = SequenceIndexer.instance.GetSequence(itemData.timeline);
            if (!sequence.isValid || !sequence.hasGameObject || sequence.isPrefabRoot)
                return false;

            return true;
        }

        bool CanDeleteSelection()
        {
            if (inPlaymode)
                return false;

            foreach (var index in selectedIndices)
            {
                var itemData = GetItemDataForIndex(index);
                if (itemData.timeline == null)
                    continue;

                var sequence = SequenceIndexer.instance.GetSequence(itemData.timeline);
                if (sequence.isValid && !sequence.hasGameObject || (sequence.isPrefabRoot && inPrefabStage))
                    return false;
            }

            return true;
        }

        protected override void RegisterEvents()
        {
            base.RegisterEvents();
            RegisterCallback<SearchEvent>(OnSearched);

            // Ensure to reflect selections in all views.
            SelectionUtility.sequenceSelectionChanged += OnSequenceSelectionChanged;

            // Add or remove tree view items when sequences are created or deleted from API.
            HierarchyDataChangeVerifier.sequenceCreated += OnSequenceCreated;
            SequenceUtility.sequenceDeleted += OnSequenceDeleted;

            // Add or remove tree view items when sequences are created or deleted manually.
            SequenceIndexer.sequenceRegistered += AddItemForSequence;
            SequenceIndexer.sequenceUpdated += OnSequenceUpdated;
            MasterSequenceUtility.masterSequencesRemoved += OnMasterSequencesRemoved;

            // Ensure the UI refresh to reflect invalid sequences or unloaded sequences.
            SequenceIndexer.validityChanged += RefreshItems;
            SequenceIndexer.sequencesRemoved += OnSequencesRemoved;
        }

        protected override void UnregisterEvents()
        {
            base.UnregisterEvents();
            UnregisterCallback<SearchEvent>(OnSearched);

            SelectionUtility.sequenceSelectionChanged -= OnSequenceSelectionChanged;
            HierarchyDataChangeVerifier.sequenceCreated -= OnSequenceCreated;
            SequenceUtility.sequenceDeleted -= OnSequenceDeleted;
            SequenceIndexer.sequenceRegistered -= AddItemForSequence;
            SequenceIndexer.sequenceUpdated -= OnSequenceUpdated;
            MasterSequenceUtility.masterSequencesRemoved -= OnMasterSequencesRemoved;
            SequenceIndexer.validityChanged -= RefreshItems;
            SequenceIndexer.sequencesRemoved -= OnSequencesRemoved;
        }

        void OnSearched(SearchEvent evt)
        {
            m_SearchQuery = evt.query;

            if (string.IsNullOrEmpty(evt.query))
                ResetSearchFilter();
            else
                ApplySearchFilter();
        }

        protected override void OnSelectionChanged(IEnumerable<object> objects)
        {
            if (m_PreventSelectionLoop || !objects.Any())
            {
                m_PreventSelectionLoop = false;
                return;
            }

            // Select the first index if any.
            var itemData = objects.First() as SequenceNode;
            if (itemData != null && itemData.timeline != null)
                SelectionUtility.TrySelectSequenceWithoutNotify(itemData.timeline);
        }

        void OnSequenceSelectionChanged()
        {
            var sequence = SelectionUtility.activeSequenceSelection;
            if (sequence == null)
                return;

            foreach (var id in viewController.GetAllItemIds())
            {
                var itemData = GetItemDataForId(id);
                if (itemData.timeline != sequence)
                    continue;

                m_PreventSelectionLoop = true;
                SetSelectionById(id);
                break;
            }
        }

        void OnSequenceCreated(TimelineAsset timeline)
        {
            var sequence = SequenceIndexer.instance.GetSequence(timeline);
            AddItemForSequence(sequence);
        }

        void AddItemForSequence(SequenceNode sequence)
        {
            // When a search is active it's safer to reapply the filter and rebuild the tree than selectively update it.
            if (!string.IsNullOrEmpty(m_SearchQuery))
            {
                ApplySearchFilter();
                return;
            }

            var parentId = -1;
            if (sequence.parent != null)
                parentId = sequence.parent.timeline.GetHashCode();

            var childIndex = GetChildIndex(sequence, parentId);

            var id = sequence.timeline.GetHashCode();
            var item = new TreeViewItemData<SequenceNode>(id, sequence);
            AddItem(item, parentId, childIndex);

            viewController.ExpandItem(parentId, false);
            SetSelectionById(id);
        }

        void OnSequenceDeleted()
        {
            var allIds = viewController.GetAllItemIds().ToArray();
            foreach (var id in allIds)
            {
                var itemData = GetItemDataForId(id);
                if (itemData == null || itemData.timeline == null)
                {
                    viewController.TryRemoveItem(id, false);
                }
            }

            RebuildTree();
            RefreshItems();
        }

        void OnSequenceUpdated(SequenceNode sequence)
        {
            // When a search is active it's safer to reapply the filter and rebuild the tree than selectively update it.
            if (!string.IsNullOrEmpty(m_SearchQuery))
            {
                ApplySearchFilter();
                return;
            }

            var id = sequence.timeline.GetHashCode();
            var parentId = viewController.GetParentId(id);
            var wasExpanded = viewController.IsExpanded(id);

            var removeSuccess = viewController.TryRemoveItem(id, false);
            var childIndex = GetChildIndex(sequence, parentId);

            var newItemData = GenerateDataItem(sequence);

            AddItem(newItemData, parentId, childIndex);
            // Check if the item being removed was expanded, if true, we re-expand the new copy.
			if (wasExpanded)
				viewController.ExpandItem(newItemData.id, false);
        }

        void OnSequencesRemoved()
        {
            RemoveBrokenItems();
            RebuildTree();
            RefreshItems();
        }

        void RemoveBrokenItems()
        {
            var rootItemIds = viewController.GetRootItemIds().ToArray();
            foreach (var id in rootItemIds)
            {
                var itemData = GetItemDataForId(id);
                if (itemData == null)
                    continue;

                if (itemData.timeline == null)
                    viewController.TryRemoveItem(id, false);

                else
                    RemoveBrokenChildrenItems(id);
            }
        }

        void RemoveBrokenChildrenItems(int id)
        {
            var childrenIds = viewController.GetChildrenIds(id).ToArray();
            foreach (var childId in childrenIds)
            {
                var itemData = GetItemDataForId(childId);
                if (itemData.timeline == null)
                    TryRemoveChildren(childId);

                else
                    RemoveBrokenChildrenItems(childId);
            }
        }

        void TryRemoveChildren(int id)
        {
            var childrenIds = viewController.GetChildrenIds(id).ToArray();
            foreach (var childId in childrenIds)
                viewController.TryRemoveItem(childId, false);
        }

        void OnMasterSequencesRemoved()
        {
            var legacyMasterSequences = MasterSequenceUtility.GetLegacyMasterSequences().ToList();
            var didRemoveItem = false;
            var rootItemIds = viewController.GetRootItemIds().ToArray();

            foreach (var id in rootItemIds)
            {
                var itemData = GetItemDataForId(id);

                if (!legacyMasterSequences.Exists(masterSequence => masterSequence.masterTimeline == itemData.timeline))
                    didRemoveItem |= viewController.TryRemoveItem(id, false);
            }

            if (didRemoveItem)
            {
                RebuildTree();
                RefreshItems();
            }
        }

        void InitializeRootItems()
        {
            var rootItems = GenerateDataTree();
            SetRootItems(rootItems);
        }

        List<TreeViewItemData<SequenceNode>> GenerateDataTree()
        {
            var rootItems = new List<TreeViewItemData<SequenceNode>>();

            foreach (var legacyMasterSequence in MasterSequenceUtility.GetLegacyMasterSequences())
            {
                var masterSequence = SequenceIndexer.instance.GetSequence(legacyMasterSequence.rootSequence.timeline);
                // The legacy data might return a MasterSequence that doesn't have a TimelineAsset.
                // In this case, the SequenceIndexer will always return null, we have to skip it as there's no
                // SequenceNode associated to it.
                if (masterSequence != null)
                    rootItems.Add(GenerateDataItem(masterSequence));
            }

            return rootItems;
        }

        TreeViewItemData<SequenceNode> GenerateDataItem(SequenceNode sequence)
        {
            var id = sequence.timeline.GetHashCode();
            var childItems = new List<TreeViewItemData<SequenceNode>>();

            foreach (var child in sequence.children)
                childItems.Add(GenerateDataItem(child));

            foreach (var invalidChild in sequence.GetEmptyClips())
                childItems.Add(GenerateDataItem(invalidChild, sequence, id));

            childItems.Sort((x, y) => m_Comparer.Compare(x.data, y.data));
            return new TreeViewItemData<SequenceNode>(id, sequence, childItems);
        }

        TreeViewItemData<SequenceNode> GenerateDataItem(KeyValuePair<TimelineClip, GameObject> clip, SequenceNode parent, int parentId)
        {
            var id = parentId + clip.Key.GetHashCode();
            var sequence = new SequenceNode {editorialClip = clip.Key, gameObject = clip.Value, parent = parent};
            return new TreeViewItemData<SequenceNode>(id, sequence);
        }

        SequenceNode GetItemDataForId(int id)
        {
            return GetItemDataForId<SequenceNode>(id);
        }

        SequenceNode GetItemDataForIndex(int index)
        {
            return GetItemDataForIndex<SequenceNode>(index);
        }

        internal void BeginItemCreation(int parentId = -1)
        {
            BeginItemCreation<SequenceNode>(parentId);
        }

        int GetChildIndex(SequenceNode sequence, int parentId)
        {
            var idList = parentId == -1 ? GetRootIds() : viewController.GetChildrenIds(parentId);

            foreach (var siblingId in idList)
            {
                if (siblingId == itemCreationId)
                    continue;

                var siblingSequence = GetItemDataForId(siblingId);

                if (siblingSequence == null)
                    continue;

                if (m_Comparer.Compare(sequence, siblingSequence) < 0)
                {
                    return viewController.GetChildIndexForId(siblingId);
                }
            }

            return -1;
        }

        void ApplySearchFilter()
        {
            var rootItems = GenerateDataTree()
                .SelectMany(TreeViewUtilities.TraverseItemData)
                .Where(item => SearchUtility.DoesTextMatchQuery(item.data.GetDisplayName(), m_SearchQuery))
                .ToList();

            SetRootItems(rootItems);
            Rebuild();
        }

        void ResetSearchFilter()
        {
            var selectedItemIds = selectedIndices.Select(viewController.GetIdForIndex).ToArray();

            var rootItems = GenerateDataTree();
            SetRootItems(rootItems);
            Rebuild();

            // When searching you can select an item that was previously buried in the tree view hierarchy.
            // We want to show selected items in an expanded state.
            foreach (var id in selectedItemIds)
                this.ExpandItemParents(id);
        }

        static int GetDepthOfItem(SequenceNode sequence)
        {
            return sequence.parent == null ? 0 : GetDepthOfItem(sequence.parent) + 1;
        }

        int GetDepthOfItemBeingCreated(int index)
        {
            var parentId = GetParentIdForIndex(index);

            if (parentId == -1)
                return 0;

            var parentSequence = GetItemDataForId(parentId);
            return GetDepthOfItem(parentSequence) + 1;
        }
    }
}
