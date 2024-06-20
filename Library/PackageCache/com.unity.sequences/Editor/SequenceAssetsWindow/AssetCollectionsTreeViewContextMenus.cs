using System.Linq;
using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    partial class AssetCollectionsTreeView
    {
        void PopulateContextMenuForAssetCollection(DropdownMenu menu, int index)
        {
            if (menu.MenuItems().Any())
                return;

            menu.AppendAction("Create Sequence Asset", CreateSequenceAsset, DropdownMenuAction.AlwaysEnabled, index);
            menu.AppendAction("Delete Sequence Assets", DeleteSelectedSequenceAssetItems, DeleteSelectedSequenceAssetActionStatus, index);
        }

        void PopulateContextMenuForSequenceAsset(DropdownMenu menu, int index)
        {
            if (menu.MenuItems().Any())
                return;

            menu.AppendAction("Create Variant", CreateSequenceAssetVariant, DropdownMenuAction.AlwaysEnabled, index);
            menu.AppendSeparator();
            menu.AppendAction("Open", OpenSequenceAsset, DropdownMenuAction.AlwaysEnabled, index);
            menu.AppendAction("Rename", RenameSequenceAsset, DropdownMenuAction.AlwaysEnabled, index);
            menu.AppendAction("Delete", DeleteSelectedSequenceAssetItems, DropdownMenuAction.AlwaysEnabled, index);
        }

        void PopulateContextMenuForSequenceAssetVariant(DropdownMenu menu, int index)
        {
            if (menu.MenuItems().Any())
                return;

            menu.AppendAction("Open", OpenSequenceAsset, DropdownMenuAction.AlwaysEnabled, index);
            menu.AppendAction("Rename", RenameSequenceAsset, DropdownMenuAction.AlwaysEnabled, index);
            menu.AppendAction("Duplicate", DuplicateSequenceAssetVariant, DropdownMenuAction.AlwaysEnabled, index);
            menu.AppendAction("Delete", DeleteSelectedSequenceAssetItems, DropdownMenuAction.AlwaysEnabled, index);
        }

        void PopulateContextMenuForMultiSelection(DropdownMenu menu)
        {
            menu.AppendAction("Delete selected", DeleteSelectedSequenceAssetItems,  DeleteSelectedSequenceAssetActionStatus, selectedIndices.ToArray());
        }

        void CreateSequenceAsset(DropdownMenuAction action)
        {
            var index = (int)action.userData;
            var id = GetIdForIndex(index);
            BeginItemCreation<AssetCollectionTreeViewItem>(id);
        }

        void CreateSequenceAssetVariant(DropdownMenuAction action)
        {
            var index = (int)action.userData;
            BeginItemCreation<AssetCollectionTreeViewItem>(viewController.GetIdForIndex(index));
        }

        void OpenSequenceAsset(DropdownMenuAction action)
        {
            var index = (int)action.userData;
            var data = GetItemDataForIndex<AssetCollectionTreeViewItem>(index);
            AssetDatabase.OpenAsset(data.asset);
        }

        void RenameSequenceAsset(DropdownMenuAction action)
        {
            BeginRenameAtIndex((int)action.userData);
        }

        void DuplicateSequenceAssetVariant(DropdownMenuAction action)
        {
            var index = (int)action.userData;
            var data = GetItemDataForIndex<AssetCollectionTreeViewItem>(index);

            SequenceAssetUtility.DuplicateVariant(data.asset);
        }

        void DeleteSelectedSequenceAssetItems(DropdownMenuAction action)
        {
            DeleteSelectedItems();
        }

        DropdownMenuAction.Status DeleteSelectedSequenceAssetActionStatus(DropdownMenuAction action)
        {
            var items = GetSelectedItems<AssetCollectionTreeViewItem>().ToList();

            if (items.Any(item => !IsItemACollection(item.data)))
                return DropdownMenuAction.Status.Normal;

            return items.All(item => !item.children.Any()) ?
                DropdownMenuAction.Status.Disabled :
                DropdownMenuAction.Status.Normal;
        }
    }
}
