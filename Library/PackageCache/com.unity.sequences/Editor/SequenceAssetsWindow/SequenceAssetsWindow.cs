using UnityEngine;
using UnityEngine.Sequences;
using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    [PackageHelpURL("sequence-assets-window")]
    internal class SequenceAssetsWindow : TreeViewEditorWindow<AssetCollectionsTreeView>
    {
        protected override void SetupView()
        {
            base.SetupView();

            titleContent = new GUIContent(
                "Sequence Assets",
                IconUtility.LoadIcon("CollectionType/CustomType", IconUtility.IconType.UniqueToSkin));
        }

        protected override string GetAddMenuTooltip()
        {
            return "Create a new Sequence Asset";
        }

        protected override void PopulateAddMenu(DropdownMenu menu, bool contextual = false)
        {
            foreach (string type in CollectionType.instance.types)
            {
                menu.AppendAction(
                    $"Create {type}",
                    CreateSequenceAssetAction,
                    DropdownMenuAction.AlwaysEnabled,
                    type);
            }
        }

        void CreateSequenceAssetAction(DropdownMenuAction action)
        {
            string type = action.userData as string;
            treeView.BeginSequenceAssetCreation(type);
        }
    }
}
