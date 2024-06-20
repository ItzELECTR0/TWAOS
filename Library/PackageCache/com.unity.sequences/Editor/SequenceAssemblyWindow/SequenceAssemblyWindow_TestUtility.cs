#if UNITY_INCLUDE_TESTS
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace UnityEditor.Sequences
{
    partial class SequenceAssemblyWindow
    {
        internal SequenceAssemblyInspector cachedEditor => m_CachedEditor;

        /// <summary>
        /// Tells if the SequenceAssemblyWindow is currently opened for the provided <paramref name="director"/>.
        /// </summary>
        /// <param name="director"></param>
        /// <returns></returns>
        internal bool IsInspecting(PlayableDirector director)
        {
            return (cachedEditor != null
                && cachedEditor.target == director
                && cachedEditor.rootVisualElement != null);
        }
    }

    partial class SequenceAssemblyInspector
    {
        internal List<AssetCollectionList> assetCollectionListsCache => m_AssetCollectionListsCache;
    }

    partial class SequenceAssetFoldoutItem
    {
        internal BasicSequenceAssetView basicView => m_SequenceAssetView as BasicSequenceAssetView;
        internal GameObject sourceAsset => m_SequenceAssetSource;
        internal GameObject selectedAsset => m_SequenceAssetSelected;
        internal GameObject selectedInstance => m_SequenceAssetSelectedInstance;
    }
}
#endif
