#if UNITY_INCLUDE_TESTS
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    /// <summary>
    /// Utility class to help internal tests query specific information in the AssetCollectionsTreeView.
    /// </summary>
    partial class AssetCollectionsTreeView
    {
        internal bool ContainsTreeViewItemOfName(string name)
        {
            ExpandAll();
            for (int index = viewController.GetItemsCount() - 1; index >= 0; --index)
            {
                int id = GetIdForIndex(index);
                var data = GetItemDataForId<AssetCollectionTreeViewItem>(id);

                if (data.treeViewItemType == AssetCollectionTreeViewItem.Type.Item
                    && data.asset.name == name)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the index of a matching <see cref="TreeViewItemData{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="match"></param>
        /// <returns></returns>
        int GetIndexOf<T>(Predicate<T> match)
        {
            ExpandAll();
            for (int index = viewController.GetItemsCount() - 1; index >= 0; --index)
            {
                int id = GetIdForIndex(index);
                var data = GetItemDataForId<T>(id);

                if (match(data))
                    return index;
            }
            return -1;
        }

        internal int GetIndexOfAssetCollectionOfName(string name)
        {
            return GetIndexOf<AssetCollectionTreeViewItem>(item =>
                item.treeViewItemType == AssetCollectionTreeViewItem.Type.Header
                && item.collectionName == name);
        }

        internal int GetIndexOfSequenceAsset(GameObject asset)
        {
            return GetIndexOf<AssetCollectionTreeViewItem>(item =>
                item.treeViewItemType == AssetCollectionTreeViewItem.Type.Item
                && item.asset == asset);
        }

        internal VisualElement GetVisualElementFromIndex(int index)
        {
            ExpandAll();

            var rootElement = GetRootElementForIndex(index);
            return rootElement.Q<VisualElement>(itemClassName);
        }

        internal void BeginRenameItem(int index)
        {
            BeginRenameAtIndex(index, 0);
        }

        internal void BeginItemCreation(int index)
        {
            BeginItemCreation<AssetCollectionTreeViewItem>(index);
        }

        internal GameObject GetAssetForIndex(int index)
        {
            var itemData = GetItemDataForIndex<AssetCollectionTreeViewItem>(index);

            return itemData.asset;
        }
    }
}

#endif // UNITY_INCLUDE_TESTS
