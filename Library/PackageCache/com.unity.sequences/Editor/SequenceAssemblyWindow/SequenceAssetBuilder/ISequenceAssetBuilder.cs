using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    internal interface ISequenceAssetBuilder
    {
        GameObject CreateSequenceAsset(string type);
        ISequenceAssetView CreateSequenceAssetView();
    }

    internal interface ISequenceAssetView
    {
        event Action<GameObject, GameObject> variantSelected;

        VisualElement root { get; }
        PopupField<GameObject> variantsSelector { get; }

        void Bind(GameObject sequenceAsset, PlayableDirector context);
        void GenerateVariantsDropdown(GameObject instanceSelection);
    }
}
