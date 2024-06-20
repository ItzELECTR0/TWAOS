using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    internal partial class BasicSequenceAssetView : ISequenceAssetView
    {
        static readonly string k_UXMLFilePath = "Packages/com.unity.sequences/Editor/SequenceAssemblyWindow/UIData/SequenceAssetVariantField.uxml";

        public event Action<GameObject, GameObject> variantSelected;

        GameObject m_Data;
        VisualElement m_root;
        PopupField<GameObject> m_DropDown;

        public VisualElement root => m_root;

        public PopupField<GameObject> variantsSelector => m_DropDown;

        public BasicSequenceAssetView()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_UXMLFilePath);
            m_root = visualTree.CloneTree();
            StyleSheetUtility.SetStyleSheets(root);
        }

        public void Bind(GameObject selection, PlayableDirector director)
        {
            m_Data = SequenceAssetUtility.GetSource(selection);

            VisualElement popup = root.Q<VisualElement>("variants");
            GenerateVariantsDropdown(selection);
            popup.Add(m_DropDown);

            Button menuButton = root.Q<Button>("context-menu");
            menuButton.clickable = new Clickable(() => ShowContextMenu(selection));
        }

        void ShowContextMenu(GameObject selection)
        {
            var selectionAsset = SequenceAssetUtility.GetAssetFromInstance(selection);
            SequenceAssemblyItemContextMenu.instance.Show(variantsSelector, selectionAsset);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="instanceSelection">A Sequence asset prefab instance to use as the selection for the generated dropdown.</param>
        /// <exception cref="ArgumentException">If the given instanceSelection is invalid.</exception>
        public void GenerateVariantsDropdown(GameObject instanceSelection)
        {
            var variants = SequenceAssetUtility.GetVariants(m_Data).ToList();
            variants.Sort((x, y) => x.name.CompareTo(y.name));
            variants.Insert(0, m_Data);

            var assetSelection = SequenceAssetUtility.GetAssetFromInstance(instanceSelection);

            int variantIndex;
            if (assetSelection == null)
                variantIndex = 0;
            else
                variantIndex = variants.IndexOf(assetSelection);

            if (variantIndex < 0)
                throw new ArgumentException(
                    $"Invalid selection: \"{instanceSelection.name}\" (instance ID: {instanceSelection.GetInstanceID()})" +
                    $" is not one of \"{m_Data.name}\" prefab and its variants. ");

            m_DropDown = new PopupField<GameObject>("", variants, variantIndex);
            variantsSelector.RegisterValueChangedCallback(OnVariantSelected);
            variantsSelector.formatListItemCallback = FormatChoices;
            variantsSelector.formatSelectedValueCallback = FormatSelection;
        }

        string FormatChoices(GameObject choice)
        {
            if (choice == m_Data)
                return "Original";

            return choice.name;
        }

        string FormatSelection(GameObject selection)
        {
            if (selection == m_Data)
                return "Original";

            return selection.name;
        }

        void OnVariantSelected(ChangeEvent<GameObject> newValue)
        {
            variantSelected?.Invoke(newValue.previousValue, newValue.newValue);

            // Remove any invalid item from the list by rebuilding the list
            // Note if the user cancel the variant change, the variantsSelector might change back to the previous value
            var popup = root.Q<VisualElement>("variants");
            popup.Remove(variantsSelector);
            GenerateVariantsDropdown(variantsSelector.value);
            popup.Add(variantsSelector);
        }
    }
}
