using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Sequences;
using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    partial class AssetCollectionsTreeView : SequencesTreeView
    {
        static class Styles
        {
            internal static readonly Dictionary<string, string> itemIconClassNames = new Dictionary<string, string>()
            {
                { "Character", itemIconClassName + "-character" },
                { "Fx", itemIconClassName + "-fx" },
                { "Lighting", itemIconClassName + "-lighting" },
                { "Photography", itemIconClassName + "-photography" },
                { "Prop", itemIconClassName + "-prop" },
                { "Set", itemIconClassName + "-set" },
                { "Audio", itemIconClassName + "-audio" }
            };

            internal static readonly string prefabItemIconClassName = itemIconClassName + "-prefab";
            internal static readonly string prefabVariantItemIconClassName = itemIconClassName + "-prefab-variant";
        }

        class AssetCollectionTreeViewItem
        {
            internal enum Type
            {
                Header,
                Item
            }

            internal Type treeViewItemType { get; } = Type.Item;
            internal string collectionName;
            internal GameObject asset;

            internal AssetCollectionTreeViewItem(Type type, string name, GameObject prefab = null)
            {
                treeViewItemType = type;
                collectionName = name;
                asset = prefab;
            }

            internal string GetItemText()
            {
                if (treeViewItemType == Type.Header)
                    return collectionName;

                if (asset == null)
                    return "NewSequenceAsset";

                return asset.name;
            }
        }

        // Keep an indexer to assign unique ID to new TreeViewItem.
        // ID starts at 1 as the root item's ID is 0.
        [SerializeField]
        int m_IdGenerator = 1;

        public AssetCollectionsTreeView() : base()
        {
            selectionType = SelectionType.Multiple;

            var rootItems = GenerateDataTree();
            SetRootItems(rootItems);
        }

        protected override void RegisterEvents()
        {
            base.RegisterEvents();

            SequenceAssetIndexer.sequenceAssetImported += OnSequenceAssetImported;
            SequenceAssetIndexer.sequenceAssetDeleted += OnSequenceAssetDeleted;
            SequenceAssetIndexer.sequenceAssetUpdated += OnSequenceAssetUpdated;
        }

        protected override void UnregisterEvents()
        {
            base.UnregisterEvents();

            SequenceAssetIndexer.sequenceAssetImported -= OnSequenceAssetImported;
            SequenceAssetIndexer.sequenceAssetDeleted -= OnSequenceAssetDeleted;
            SequenceAssetIndexer.sequenceAssetUpdated -= OnSequenceAssetUpdated;
        }

        protected override void InitClassListAtIndex(VisualElement ve, int index)
        {
            var icon = GetIconElement(ve);
            var data = GetItemDataForIndex<AssetCollectionTreeViewItem>(index);
            string classToEnable;

            if (data != null && IsItemACollection(data))
                classToEnable = Styles.itemIconClassNames[data.collectionName];
            else if (data != null && data.treeViewItemType == AssetCollectionTreeViewItem.Type.Item)
            {
                if (SequenceAssetUtility.IsSource(data.asset))
                    classToEnable = Styles.prefabItemIconClassName;
                else
                    classToEnable = Styles.prefabVariantItemIconClassName;
            }
            else
            {
                var parent = GetItemDataForId<AssetCollectionTreeViewItem>(GetParentIdForIndex(index));
                classToEnable = IsItemACollection(parent)
                    ? Styles.prefabItemIconClassName
                    : Styles.prefabVariantItemIconClassName;
            }

            icon.EnableInClassList(classToEnable, true);
        }

        protected override void ResetClassListAtIndex(VisualElement ve, int index)
        {
            var icon = GetIconElement(ve);
            foreach (var itemClass in Styles.itemIconClassNames)
                icon.EnableInClassList(itemClass.Value, false);

            icon.EnableInClassList(Styles.prefabItemIconClassName, false);
            icon.EnableInClassList(Styles.prefabVariantItemIconClassName, false);
        }

        protected override void DoubleClicked(int index)
        {
            var data = GetItemDataForIndex<AssetCollectionTreeViewItem>(index);
            if (data.treeViewItemType == AssetCollectionTreeViewItem.Type.Item)
                AssetDatabase.OpenAsset(data.asset);
        }

        protected override void DuplicateSelectedItems()
        {
            var selectedVariantsData = selectedIndices
                .Select(GetItemDataForIndex<AssetCollectionTreeViewItem>)
                .Where(data => data is { treeViewItemType: AssetCollectionTreeViewItem.Type.Item } &&
                    PrefabUtility.IsPartOfVariantPrefab(data.asset))
                .ToArray();

            foreach (var data in selectedVariantsData)
            {
                SequenceAssetUtility.DuplicateVariant(data.asset);
            }
        }

        protected override void ContextClicked(DropdownMenu menu)
        {
            var indices = selectedIndices.ToArray();
            var data = GetItemDataForIndex<AssetCollectionTreeViewItem>(indices[0]);

            if (selectedIndices.Count() > 1)
                PopulateContextMenuForMultiSelection(menu);

            else if (IsItemACollection(data))
                PopulateContextMenuForAssetCollection(menu, indices[0]);

            else
            {
                if (SequenceAssetUtility.IsSource(data.asset))
                    PopulateContextMenuForSequenceAsset(menu, indices[0]);
                else
                    PopulateContextMenuForSequenceAssetVariant(menu, indices[0]);
            }
        }

        protected override void DeleteSelectedItems()
        {
            var items = GetSelectedItems<AssetCollectionTreeViewItem>().ToList();
            var areCollections = items.All(item => IsItemACollection(item.data));

            var sequenceAssets = GetSequenceAssetToDelete(items).ToList();
            if (sequenceAssets.Count > 0 && !UserVerifications.ValidateSequenceAssetDeletion(sequenceAssets.ToList(), areCollections))
                return;

            foreach (var sequenceAsset in sequenceAssets)
            {
                if (SequenceAssetUtility.IsSource(sequenceAsset))
                    SequenceAssetUtility.DeleteSourceAsset(sequenceAsset);
                else
                    SequenceAssetUtility.DeleteVariantAsset(sequenceAsset);
            }
        }

        protected override string GetItemTextForIndex(int index)
        {
            var itemData = GetItemDataForIndex<AssetCollectionTreeViewItem>(index);
            if (itemData != null)
                return itemData.GetItemText();

            var parentId = GetParentIdForIndex(index);
            var parent = GetItemDataForId<AssetCollectionTreeViewItem>(parentId);

            if (IsItemACollection(parent))
                return SequenceAssetUtility.GetDefaultSequenceAssetName(parent.collectionName);

            return SequenceAssetUtility.GetVariantName(parent.asset);
        }

        protected override void RenameEnded(int id, bool canceled = false)
        {
            var root = GetRootElementForId(id);
            var label = root.Q<RenameableLabel>();
            var newName = label.text;
            var itemData = GetItemDataForId<AssetCollectionTreeViewItem>(id);

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

            // An asset creation is requested from the user.
            if (itemData == null)
            {
                if (string.IsNullOrWhiteSpace(newName))
                {
                    var index = viewController.GetIndexForId(id);
                    newName = GetItemTextForIndex(index);
                }

                var parentId = viewController.GetParentId(id);

                // Remove the item used as a placeholder. OnSequenceAssetImported will be called once the
                // API is done creating the asset. That's the moment where we'll create the definitive item.
                viewController.TryRemoveItem(id, false);

                var parentItem = GetItemDataForId<AssetCollectionTreeViewItem>(parentId);
                if (IsItemACollection(parentItem))
                    SequenceAssetUtility.CreateSource(newName, parentItem.collectionName);
                else
                {
                    SequenceAssetUtility.CreateVariant(parentItem.asset, newName);
                }
            }
            else
                SequenceAssetUtility.Rename(itemData.asset, itemData.asset.name, newName);
        }

        protected override bool CanRename(int index)
        {
            var data = GetItemDataForIndex<AssetCollectionTreeViewItem>(index);
            if (data != null)
                return data.treeViewItemType == AssetCollectionTreeViewItem.Type.Item;

            // Index can lead to an invalid TreeViewDataItem when they are placeholders.
            // Ex: placeholder for a new asset until the user validates the asset name.
            return base.CanRename(index);
        }

        List<TreeViewItemData<AssetCollectionTreeViewItem>> GenerateDataTree()
        {
            var rootItems = new List<TreeViewItemData<AssetCollectionTreeViewItem>>();

            foreach (var collection in CollectionType.instance.types)
            {
                rootItems.Add(GenerateDataItem(collection));
            }

            return rootItems;
        }

        TreeViewItemData<AssetCollectionTreeViewItem> GenerateDataItem(string collection)
        {
            var sources = SequenceAssetUtility.FindAllSources(collection).ToList();
            sources.Sort((x, y) => x.name.CompareTo(y.name));

            var children = new List<TreeViewItemData<AssetCollectionTreeViewItem>>();
            foreach (var sourcePrefab in sources)
            {
                children.Add(GenerateDataItem(collection, sourcePrefab));
            }

            var itemData = new AssetCollectionTreeViewItem(AssetCollectionTreeViewItem.Type.Header, collection);
            var item = new TreeViewItemData<AssetCollectionTreeViewItem>(GetNextId(), itemData, children);

            return item;
        }

        TreeViewItemData<AssetCollectionTreeViewItem> GenerateDataItem(string collection, GameObject prefab)
        {
            var variantSources = SequenceAssetUtility.GetVariants(prefab).ToList();
            variantSources.Sort((x, y) => x.name.CompareTo(y.name));

            var variants = new List<TreeViewItemData<AssetCollectionTreeViewItem>>();
            foreach (var variant in variantSources)
            {
                var childItemData = new AssetCollectionTreeViewItem(AssetCollectionTreeViewItem.Type.Item, collection, variant);
                var childItem = new TreeViewItemData<AssetCollectionTreeViewItem>(GetNextId(), childItemData);
                variants.Add(childItem);
            }

            var itemData = new AssetCollectionTreeViewItem(AssetCollectionTreeViewItem.Type.Item, collection, prefab);
            var item = new TreeViewItemData<AssetCollectionTreeViewItem>(GetNextId(), itemData, variants);

            return item;
        }

        int GetNextId()
        {
            return m_IdGenerator++;
        }

        /// <summary>
        /// Listens for new import of Sequence Assets from the AssetDatabase.
        /// </summary>
        /// <param name="gameObject"></param>
        void OnSequenceAssetImported(GameObject gameObject)
        {
            if (GetIdFor(gameObject) > -1)
                return;

            string type = SequenceAssetUtility.GetType(gameObject);

            int parentId, childIndex;
            AssetCollectionTreeViewItem itemData;

            // Duplicated Prefab or Prefab variant created from the Project View.
            if (SequenceAssetUtility.IsSource(gameObject))
                parentId = GetIdFor(type);
            else
            {
                GameObject baseObject = SequenceAssetUtility.GetSource(gameObject);
                parentId = GetIdFor(baseObject);
            }

            itemData = new AssetCollectionTreeViewItem(AssetCollectionTreeViewItem.Type.Item, type, gameObject);
            childIndex = GetChildIndex(itemData.asset.name, parentId);

            AddItem(new TreeViewItemData<AssetCollectionTreeViewItem>(GetNextId(), itemData), parentId, childIndex);
        }

        /// <summary>
        /// Listens for deletion of Sequence Assets in the AssetDatabse.
        /// It detaches TreeView items affected by this deletion.
        /// </summary>
        void OnSequenceAssetDeleted()
        {
            // Deletion from the asset database.
            // Go over all the items and delete the ones with an invalid prefab reference.
            var ids = viewController.GetAllItemIds().ToArray();
            foreach (var id in ids)
            {
                var data = GetItemDataForId<AssetCollectionTreeViewItem>(id);

                if (IsItemACollection(data))
                    continue;

                if (data.asset == null)
                    viewController.TryRemoveItem(id, false);
            }
            RebuildTree();
            RefreshItems();
        }

        void OnSequenceAssetUpdated(GameObject sequenceAsset)
        {
            foreach (var id in viewController.GetAllItemIds())
            {
                var data = GetItemDataForId<AssetCollectionTreeViewItem>(id);
                if (IsItemACollection(data))
                    continue;

                if (data.asset == sequenceAsset)
                {
                    var parentId = viewController.GetParentId(id);
                    var childIndex = GetChildIndex(sequenceAsset.name, parentId);
                    viewController.Move(id, parentId, childIndex);
                    return;
                }
            }
        }

        protected override void OnSelectionChanged(IEnumerable<object> objects)
        {
            foreach (AssetCollectionTreeViewItem item in objects)
            {
                if (item == null || IsItemACollection(item))
                    continue;

                SelectionUtility.SetSelection(item.asset);

                // Does not support multi-selection at the moment.
                break;
            }
        }

        int GetIdFor(string assetCollection)
        {
            foreach (var rootId in viewController.GetRootItemIds())
            {
                var data = GetItemDataForId<AssetCollectionTreeViewItem>(rootId);

                if (data.collectionName == assetCollection)
                    return rootId;
            }

            return -1;
        }

        int GetIdFor(GameObject asset)
        {
            foreach (var id in viewController.GetAllItemIds())
            {
                var data = GetItemDataForId<AssetCollectionTreeViewItem>(id);
                if (IsItemACollection(data))
                    continue;

                if (data.asset == asset)
                    return id;
            }

            return -1;
        }

        int GetChildIndex(string assetName, int parentId)
        {
            var idList = viewController.GetChildrenIds(parentId);

            foreach (var siblingId in idList)
            {
                var siblingItem = GetItemDataForId<AssetCollectionTreeViewItem>(siblingId);
                if (siblingItem == null)
                    continue;

                if (assetName.CompareTo(siblingItem.asset.name) < 0)
                    return viewController.GetChildIndexForId(siblingId);
            }

            return -1;
        }

        internal void BeginSequenceAssetCreation(string assetCollection)
        {
            int id = GetIdFor(assetCollection);
            BeginItemCreation<AssetCollectionTreeViewItem>(id);
        }

        bool IsItemACollection(AssetCollectionTreeViewItem item)
        {
            return item.treeViewItemType == AssetCollectionTreeViewItem.Type.Header;
        }

        /// <summary>
        /// Return the actual list of sequence assets to delete.
        /// - If there is a collection in the selection, then we return all its child sequence asset if any.
        /// - If a sequence asset source and one or multiple of its variant are selected, we only return the source.
        /// </summary>
        IEnumerable<GameObject> GetSequenceAssetToDelete(IList<TreeViewItemData<AssetCollectionTreeViewItem>> selection)
        {
            var selectedSequenceAssets = selection.Select(item => item.data.asset).ToList();
            foreach (var item in selection)
            {
                if (!IsItemACollection(item.data))
                {
                    var sequenceAsset = item.data.asset;
                    if (SequenceAssetUtility.IsVariant(sequenceAsset))
                    {
                        // If the selected item is a Sequence Asset variant only yield it if its parent source asset is
                        // not as well in the list of asset to delete.
                        var sourceAsset = SequenceAssetUtility.GetSource(sequenceAsset);
                        if (!selectedSequenceAssets.Contains(sourceAsset))
                            yield return sequenceAsset;
                    }
                    else
                        yield return sequenceAsset;

                    continue;
                }

                // If the selected item is a collection, yield all its Sequence Asset children.
                foreach (var childItem in item.children)
                {
                    var sequenceAsset = childItem.data.asset;
                    if (!selectedSequenceAssets.Contains(sequenceAsset))
                        yield return sequenceAsset;
                }
            }
        }
    }
}
