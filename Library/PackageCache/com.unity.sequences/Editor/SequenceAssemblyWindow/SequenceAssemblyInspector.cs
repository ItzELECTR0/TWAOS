using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Sequences;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    internal partial class SequenceAssemblyInspector : Editor
    {
        class Styles
        {
            public static readonly string k_UXMLFilePath = "Packages/com.unity.sequences/Editor/SequenceAssemblyWindow/UIData/SequenceAssemblyInspector.uxml";
        }

        internal VisualElement rootVisualElement;
        protected PlayableDirector m_Director;

        List<AssetCollectionList> m_AssetCollectionListsCache;

        internal virtual void Initialize()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Styles.k_UXMLFilePath);
            rootVisualElement = visualTree.CloneTree();
            StyleSheetUtility.SetStyleSheets(rootVisualElement, "SequenceAssemblyInspector");

            m_AssetCollectionListsCache = new List<AssetCollectionList>();

            if (target == null)
                return;

            m_Director = (target as PlayableDirector);

            SetSequenceAssemblyData();

            SequenceAssetIndexer.sequenceAssetImported += OnSequenceAssetImported;
            SequenceAssetIndexer.sequenceAssetUpdated += OnSequenceAssetUpdated;
            SequenceAssetIndexer.sequenceAssetDeleted += OnSequenceAssetDeleted;
        }

        internal virtual void Unload()
        {
            SequenceAssetIndexer.sequenceAssetImported -= OnSequenceAssetImported;
            SequenceAssetIndexer.sequenceAssetUpdated -= OnSequenceAssetUpdated;
            SequenceAssetIndexer.sequenceAssetDeleted -= OnSequenceAssetDeleted;

            ClearCollectionsCache();
        }

        void ClearCollectionsCache()
        {
            foreach (var assetCollectionList in m_AssetCollectionListsCache)
                assetCollectionList.Dispose();

            m_AssetCollectionListsCache.Clear();
        }

        public override VisualElement CreateInspectorGUI()
        {
            return rootVisualElement;
        }

        void SetSequenceAssemblyData()
        {
            rootVisualElement.Bind(new SerializedObject(m_Director.gameObject));

            var instances = SequenceAssetUtility.GetInstancesInSequence(m_Director);
            SetAssetCollections(instances);
        }

        void SetAssetCollections(IEnumerable<GameObject> sequenceAssetSelections)
        {
            foreach (string collectionType in CollectionType.instance.types)
            {
                var sequenceAssetOfType = sequenceAssetSelections.Where(value => SequenceAssetUtility.GetType(value) == collectionType);
                SetAssetCollection(collectionType, sequenceAssetOfType);
            }
        }

        void OnSequenceAssetImported(GameObject sequenceAsset)
        {
            Refresh();
        }

        void OnSequenceAssetUpdated(GameObject sequenceAsset)
        {
            Refresh();
        }

        void OnSequenceAssetDeleted()
        {
            Refresh();
        }

        protected virtual void SetAssetCollection(string type, IEnumerable<GameObject> sequenceAssetSelections)
        {
            var assetCollectionList = AssetCollectionList.Get(m_Director, type, false);
            AddCollectionList(sequenceAssetSelections, assetCollectionList);
        }

        protected void AddCollectionList(IEnumerable<GameObject> sequenceAssetSelections, AssetCollectionList newAssetCollectionList)
        {
            AddSequenceAssetToCollection(sequenceAssetSelections, newAssetCollectionList);
            VisualElement assetCollectionsRoot = rootVisualElement.Q<VisualElement>("seq-asset-collections");
            assetCollectionsRoot.Add(newAssetCollectionList);
            m_AssetCollectionListsCache.Add(newAssetCollectionList);
        }

        internal void Refresh()
        {
            if (m_Director == null || (m_Director.playableAsset as TimelineAsset) == null)
                return;

            VisualElement assetCollectionsRoot = rootVisualElement.Q<VisualElement>("seq-asset-collections");
            foreach (var child in assetCollectionsRoot.Children())
            {
                var assetCollectionList = child as AssetCollectionList;
                if (assetCollectionList == null)
                    continue;

                assetCollectionList.RemoveAllItems();

                var instances = SequenceAssetUtility.GetInstancesInSequence(m_Director, assetCollectionList.collectionType);
                AddSequenceAssetToCollection(instances, assetCollectionList);
            }
        }

        protected virtual void AddSequenceAssetToCollection(IEnumerable<GameObject> sequenceAssetSelections, AssetCollectionList assetCollectionList)
        {
            foreach (var selection in sequenceAssetSelections)
                assetCollectionList.AddSequenceAssetSelection<SequenceAssetFoldoutItem>(selection);
        }

        internal void SelectPlayableDirector()
        {
            if (m_Director != null)
                Selection.activeGameObject = m_Director.gameObject;
        }
    }
}
