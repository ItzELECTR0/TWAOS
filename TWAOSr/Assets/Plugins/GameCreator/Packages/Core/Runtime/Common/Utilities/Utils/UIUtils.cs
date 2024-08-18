using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public static class UIUtils
    {
        public const int LAYER_UI = 5;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static GameObject Instantiate(GameObject prefab, RectTransform parent)
        {
            if (prefab == null) return null;
            if (parent == null) return null;
            
            GameObject instance = Object.Instantiate(prefab, parent);

            RectTransform transformPrefab = prefab.GetComponent<RectTransform>();
            RectTransform transformInstance = instance.GetComponent<RectTransform>();
                
            transformInstance.anchorMin = transformPrefab.anchorMin;
            transformInstance.anchorMax = transformPrefab.anchorMax;
            transformInstance.pivot = transformPrefab.pivot;
            transformInstance.offsetMin = transformPrefab.offsetMin;
            transformInstance.offsetMax = transformPrefab.offsetMax;
            transformInstance.anchoredPosition = transformPrefab.anchoredPosition;

            return instance;
        }
    }
}