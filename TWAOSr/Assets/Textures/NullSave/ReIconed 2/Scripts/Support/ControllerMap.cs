using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NullSave
{
    [CreateAssetMenu(menuName = "NullSave/ReIconed 2/Controller Map", order = 0)]
    public class ControllerMap : ScriptableObject
    {

        #region Variables

        public List<string> compatibleDevices;
        [Tooltip("Use this controller when no other compatible map can be found?")] public bool isFallback;
        [Tooltip("Is this a controller not listed below?")] public bool isCustom;
        [Tooltip("Guid associated with this custom controller")] public string customGuid;
        [Tooltip("Text Mesh Pro sprite asset to associate with this controller.")] public TMP_SpriteAsset tmpSpriteAsset;
        public List<InputMap> inputMaps;

        #endregion

    }
}