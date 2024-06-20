using System.Collections.Generic;
using UnityEngine;
using PopupField = UnityEngine.UIElements.PopupField<UnityEngine.GameObject>;

namespace UnityEditor.Sequences
{
    /// <summary>
    /// The context menu for each Sequence Asset item of the Sequence Assembly window.
    /// </summary>
    class SequenceAssemblyItemContextMenu
    {
        static SequenceAssemblyItemContextMenu m_Instance;

        public static SequenceAssemblyItemContextMenu instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new SequenceAssemblyItemContextMenu();

                return m_Instance;
            }
        }

        GenericMenu m_Menu;

        GameObject sourceAsset { get; set; }
        GameObject selectionAsset { get; set; }
        GameObject selectionInstance { get; set; }
        PopupField variantSelector { get; set; }

        public void Show(PopupField selector, GameObject target)
        {
            Initialize(selector, target);
            m_Menu = new GenericMenu();

            AddItem("Create new variant", true, CreateNewVariant);

            bool isVariant = sourceAsset != selectionAsset;
            AddItem("Duplicate current variant", isVariant, DuplicateVariant);
            AddItem("Delete current variant", isVariant, DeleteVariant);

            m_Menu.ShowAsContext();
        }

        void Initialize(PopupField selector, GameObject selection)
        {
            selectionInstance = selection;
            selectionAsset = SequenceAssetUtility.GetAssetFromInstance(selection);
            sourceAsset = SequenceAssetUtility.GetSource(this.selectionAsset);
            variantSelector = selector;
        }

        void AddItem(string content, bool enabled = true, GenericMenu.MenuFunction func = null)
        {
            if (enabled)
                m_Menu.AddItem(new GUIContent(content), false, func);
            else
                m_Menu.AddDisabledItem(new GUIContent(content), false);
        }

        void CreateNewVariant()
        {
            SequenceAssetUtility.CreateVariant(sourceAsset);
        }

        void DuplicateVariant()
        {
            SequenceAssetUtility.DuplicateVariant(selectionAsset);
        }

        void DeleteVariant()
        {
            if (!UserVerifications.ValidateSequenceAssetDeletion(new List<GameObject> {selectionAsset}))
                return;

            variantSelector.value = sourceAsset;
            SequenceAssetUtility.DeleteVariantAsset(selectionAsset);
        }
    }
}
