using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    internal class SequenceAssemblyPlayModeInspector : SequenceAssemblyInspector
    {
        internal override void Initialize()
        {
            base.Initialize();

            var playmodeMessage = new Label("Read-only in Play mode.");
            playmodeMessage.AddToClassList("seq-warning-label");
            rootVisualElement.Insert(0, playmodeMessage);
        }

        protected override void SetAssetCollection(string type, IEnumerable<GameObject> sequenceAssetSelections)
        {
            var assetCollectionList = AssetCollectionList.Get(m_Director, type, true);
            AddCollectionList(sequenceAssetSelections, assetCollectionList);
        }

        protected override void  AddSequenceAssetToCollection(IEnumerable<GameObject> sequenceAssetSelections, AssetCollectionList assetCollectionList)
        {
            foreach (var selection in sequenceAssetSelections)
                assetCollectionList.AddSequenceAssetSelection<SequenceAssetInstanceItem>(selection);
        }
    }
}
