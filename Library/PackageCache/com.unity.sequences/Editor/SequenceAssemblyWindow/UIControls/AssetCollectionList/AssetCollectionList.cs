using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Pool;
using UnityEngine.Sequences;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    class AssetCollectionList : VisualElement
    {
        static readonly string k_UIBasePath = "Packages/com.unity.sequences/Editor/SequenceAssemblyWindow/UIControls/AssetCollectionList";
        static readonly string k_UXMLFilePath = Path.Combine(k_UIBasePath, "AssetCollectionList.uxml");
        static readonly string k_USSFilePath = Path.Combine(k_UIBasePath, "AssetCollectionList.uss");

        static readonly ObjectPool<AssetCollectionList> s_Pool = new(
            () => new AssetCollectionList(),
            defaultCapacity : CollectionType.instance.types.Count);

        internal string collectionType { get; private set; }

        readonly SelectableScrollView m_ContentContainer;
        readonly List<GameObject> m_Items = new();
        readonly VisualElement m_Footer;
        readonly Label m_Label;

        SequenceAssetAddMenu m_AddMenu;
        PlayableDirector m_Director;
        TimelineAsset m_Timeline;

        AssetCollectionList()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_UXMLFilePath);
            visualTree.CloneTree(this);

            StyleSheetUtility.SetStyleSheets(this);
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_USSFilePath));

            m_ContentContainer = this.Q<SelectableScrollView>("seq-sequence-asset-list");
            m_Footer = this.Q("seq-list-footer");
            m_Label = this.Q<Label>("seq-asset-collection");

            var addButton = this.Q<Button>("seq-sequence-asset-add");
            addButton.clicked += OpenAddMenu;

            var removeButton = this.Q<Button>("seq-sequence-asset-remove");
            removeButton.clicked += RemoveSelectedSequenceAsset;
        }

        public void Dispose()
        {
            for (int i = 0; i < m_ContentContainer.childCount; ++i)
                (m_ContentContainer[i] as SelectableScrollViewItem).Dispose();

            RemoveAllItems();
            s_Pool.Release(this);
        }

        public void RemoveAllItems()
        {
            m_Items.Clear();
            m_ContentContainer.Refresh();
        }

        public void AddSequenceAssetSelection<T>(GameObject selection) where T : SelectableScrollViewItem, new()
        {
            m_Items.Add(selection);
            var foldout = new T();
            foldout.BindItem(m_Director, m_Items[m_Items.Count - 1]);
            m_ContentContainer.AddItem(foldout);
        }

        void RemoveSelectedSequenceAsset()
        {
            if (m_ContentContainer.selectedIndex < 0)
                return;

            GameObject selection = m_Items[m_ContentContainer.selectedIndex];
            SequenceAssetUtility.RemoveFromSequence(selection, m_Timeline.FindDirector());
        }

        void CreateSequenceAssetAndInstantiate(string type)
        {
            var builder = SequenceAssetBuilder.GetBuilder(typeof(GameObject));

            var prefab = builder.CreateSequenceAsset(type);
            SequenceAssetUtility.InstantiateInSequence(prefab, m_Timeline.FindDirector());
        }

        void InstantiateSequenceAsset(GameObject prefab)
        {
            SequenceAssetUtility.InstantiateInSequence(prefab, m_Timeline.FindDirector());
        }

        void OpenAddMenu()
        {
            if (m_AddMenu == null)
                m_AddMenu = new SequenceAssetAddMenu(collectionType, InstantiateSequenceAsset, CreateSequenceAssetAndInstantiate);

            m_AddMenu.Show();
        }

        public static AssetCollectionList Get(PlayableDirector director, string type, bool playMode)
        {
            var assetCollectionList = s_Pool.Get();
            assetCollectionList.collectionType = type;
            assetCollectionList.m_Director = director;
            assetCollectionList.m_Footer.style.display = playMode ? DisplayStyle.None : DisplayStyle.Flex;
            assetCollectionList.m_Label.text = type;
            assetCollectionList.m_Timeline = director.playableAsset as TimelineAsset;
            return assetCollectionList;
        }
    }
}
