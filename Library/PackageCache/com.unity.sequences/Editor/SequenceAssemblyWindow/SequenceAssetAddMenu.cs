using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.Sequences
{
    partial class SequenceAssetAddMenu
    {
        GenericMenu m_Menu;
        List<GameObject> m_SequenceAssets;
        readonly string m_Type;
        readonly Action<GameObject> m_UserClickedOnAddSequenceAsset;
        readonly Action<string> m_UserClickedOnCreateSequenceAsset;

        public SequenceAssetAddMenu(string type, Action<GameObject> userClickedOnAddSequenceAsset, Action<string> userClickedOnCreateSequenceAsset)
        {
            m_Type = type;
            m_UserClickedOnAddSequenceAsset = userClickedOnAddSequenceAsset;
            m_UserClickedOnCreateSequenceAsset = userClickedOnCreateSequenceAsset;

            SequenceAssetIndexer.sequenceAssetDeleted += OnSequenceAssetDeleted;
            SequenceAssetIndexer.sequenceAssetImported += OnSequenceAssetImported;
            SequenceAssetIndexer.sequenceAssetUpdated += OnSequenceAssetUpdated;
        }

        public void Show()
        {
            if (m_Menu == null)
                Populate();

            m_Menu.ShowAsContext();
        }

        void OnSequenceAssetDeleted()
        {
            // We don't know if this menu included the deleted sequence asset, so force both the sequences list and the
            // menu to repopulate when Show is next called.
            m_Menu = null;
            m_SequenceAssets = null;
        }

        void OnSequenceAssetImported(GameObject sequenceAsset)
        {
            if (SequenceAssetUtility.GetType(sequenceAsset) != m_Type)
                return;

            // If we have yet to populate the sequence assets list (i.e. Show has never been called), we can skip
            // updating the list and instead wait for Populate to call SequenceAssetUtility.FindAllSources.
            if (m_SequenceAssets == null)
                return;

            // Ignore sequence asset variants.
            // The current UX requires that you first add a source sequence asset before choosing a variant.
            if (!SequenceAssetUtility.IsSource(sequenceAsset))
                return;

            m_SequenceAssets.Add(sequenceAsset);

            // Force the menu to repopulate when Show is next called.
            m_Menu = null;
        }

        void OnSequenceAssetUpdated(GameObject sequenceAsset)
        {
            if (SequenceAssetUtility.GetType(sequenceAsset) != m_Type)
                return;

            // Force the menu to repopulate when Show is next called.
            // This is necessary because the names of the sequence assets may have changed.
            m_Menu = null;
        }

        void Populate()
        {
            if (m_SequenceAssets == null)
                m_SequenceAssets = SequenceAssetUtility.FindAllSources(m_Type).ToList();

            m_SequenceAssets.Sort((x, y) => x.name.CompareTo(y.name));

            m_Menu = new GenericMenu { allowDuplicateNames = true };

            foreach (var sequenceAsset in m_SequenceAssets)
                m_Menu.AddItem(new GUIContent(sequenceAsset.name), false, AddSequenceAsset, sequenceAsset);

            if (m_Menu.GetItemCount() > 0)
                m_Menu.AddSeparator("/");

            m_Menu.AddItem(new GUIContent("Create Sequence Asset"), false, CreateSequenceAsset);
        }

        void AddSequenceAsset(object sequenceAsset)
        {
            var sequenceAssetGo = sequenceAsset as GameObject;
            m_UserClickedOnAddSequenceAsset.Invoke(sequenceAssetGo);
        }

        void CreateSequenceAsset()
        {
            m_UserClickedOnCreateSequenceAsset.Invoke(m_Type);
        }
    }
}
