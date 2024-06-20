using System;
using System.IO;
using UnityEngine;

namespace UnityEditor.Sequences
{
    /// <summary>
    ///
    /// </summary>
    [FilePath("Library/QuickSequenceAsset.index", FilePathAttribute.Location.ProjectFolder)]
    class SequenceAssetIndexer : ScriptableSingleton<SequenceAssetIndexer>
    {
        [InitializeOnLoadMethod]
        static void InitializeOnLoad()
        {
            string indexerFilePath = Path.Combine(Application.dataPath, "..", GetFilePath());
            if (!File.Exists(indexerFilePath))
            {
                // Delay the initialize call because operations with AssetDatabase must wait the Editor to refresh at least once.
                EditorApplication.delayCall += InitializeWithExistingData;
            }
            else
            {
                // Remove null references. Might happen if files were removed directly from the filesystem.
                instance.PruneDeletedSequenceAsset();
            }
        }

        /// <summary>
        /// Runs a scan pass on the data every time the Editor starts.
        /// When no indexer already exists, initialize a new index.
        /// When one exists, remove all potential null references from the index.
        /// </summary>
        internal static void InitializeWithExistingData()
        {
            foreach (GameObject go in SequencesAssetDatabase.FindAllSequenceAssets())
                instance.AddSequenceAsset(go);
        }

        [Serializable]
        internal class Index
        {
            public GameObject mainPrefab;
            public GameObject[] variants = new GameObject[0];
        }

        /// <summary>
        /// Event triggered when a new Sequence Asset or Sequence Asset Variant has been imported.
        /// </summary>
        internal static event Action<GameObject> sequenceAssetImported;

        /// <summary>
        /// Event triggered when a Sequence Asset or Sequence Asset Variant has been deleted from the
        /// asset database.
        /// </summary>
        internal static event Action sequenceAssetDeleted;

        /// <summary>
        /// Event triggered when a Sequence Asset has been renamed or moved into another folder.
        /// </summary>
        internal static event Action<GameObject> sequenceAssetUpdated;

        // TODO: Use ISerializationCallbackReceiver to build a data structure easier and faster to access.
        [SerializeField] Index[] m_Indexes = new Index[0];

        internal Index[] indexes => m_Indexes;

        public bool IsRegistered(GameObject prefab)
        {
            if (SequenceAssetUtility.IsSource(prefab))
            {
                return GetIndexOf(prefab) >= 0;
            }
            else if (SequenceAssetUtility.IsVariant(prefab))
            {
                GameObject source = SequenceAssetUtility.GetSource(prefab);
                if (source == null)
                    return false;

                int index = GetIndexOf(source);
                if (index < 0)
                    return false;

                return (ArrayUtility.Contains<GameObject>(indexes[index].variants, prefab));
            }

            return false;
        }

        public void AddSequenceAsset(GameObject prefab)
        {
            if (SequenceAssetUtility.IsSource(prefab))
            {
                AddSequenceAssetSource(prefab);
            }
            else if (SequenceAssetUtility.IsVariant(prefab))
            {
                GameObject source = SequenceAssetUtility.GetSource(prefab);
                if (source == null)
                    return;

                AddSequenceAssetVariant(source, prefab);
            }

            Save();
        }

        public void UpdateSequenceAsset(GameObject prefab)
        {
            sequenceAssetUpdated?.Invoke(prefab);
        }

        public void PruneDeletedSequenceAsset()
        {
            var isIndexerChanged = false;
            foreach (Index item in m_Indexes)
            {
                if (item.mainPrefab == null)
                {
                    ArrayUtility.Remove(ref m_Indexes, item);
                    isIndexerChanged = true;
                    continue;
                }

                for (int i = item.variants.Length - 1; i >= 0; --i)
                {
                    if (item.variants[i] == null)
                    {
                        ArrayUtility.RemoveAt(ref item.variants, i);
                        isIndexerChanged = true;
                    }
                }
            }

            if (isIndexerChanged)
            {
                sequenceAssetDeleted?.Invoke();
                Save();
            }
        }

        public void Save()
        {
            instance.Save(true);
        }

        public int GetIndexOf(GameObject prefab)
        {
            return ArrayUtility.FindIndex(m_Indexes, (i) => i.mainPrefab == prefab);
        }

        int AddSequenceAssetSource(GameObject prefab)
        {
            int i = GetIndexOf(prefab);
            if (i >= 0)
                return i;
            ArrayUtility.Add(ref m_Indexes, new Index() { mainPrefab = prefab });
            sequenceAssetImported?.Invoke(prefab);
            return m_Indexes.Length - 1;
        }

        void AddSequenceAssetVariant(GameObject source, GameObject variant)
        {
            int i = GetIndexOf(source);
            if (i < 0)
                i = AddSequenceAssetSource(source);

            Index data = m_Indexes[i];
            if (ArrayUtility.Contains(data.variants, variant))
                return;

            ArrayUtility.Add(ref data.variants, variant);
            sequenceAssetImported?.Invoke(variant);
        }
    }
}
