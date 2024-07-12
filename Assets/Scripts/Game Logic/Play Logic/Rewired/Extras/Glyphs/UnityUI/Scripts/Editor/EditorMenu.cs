// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#if UNITY_2017 || UNITY_2018 || UNITY_2019 || UNITY_2020 || UNITY_2021 || UNITY_2022 || UNITY_2023 || UNITY_6000 || UNITY_6000_0_OR_NEWER
#define UNITY_2017_PLUS
#endif

namespace Rewired.Glyphs.UnityUI.Editor {
    using System;

    public static class EditorMenu {

        private const string logPrefix = "Rewired: ";
        private const string gameObjectMenuCreate = "GameObject/Create Other/Rewired/Glyphs/Unity UI/";
        private const string helpMenuCreate = "Window/Rewired/Create/Glyphs/Unity UI/";

        [UnityEditor.MenuItem(gameObjectMenuCreate + "Unity UI Player Controller Element Glyph")]
        [UnityEditor.MenuItem(helpMenuCreate + "Unity UI Player Controller Element Glyph")]
        public static void CreateControllerElementGlyphProvider() {
            
            UnityEngine.Transform parent = null;

            if (UnityEditor.Selection.activeGameObject != null && UnityEditor.Selection.gameObjects.Length == 1) {
                parent = UnityEditor.Selection.activeGameObject.transform;
            }

            UnityEngine.GameObject go = new UnityEngine.GameObject("Unity UI Player Controller Element Glyph", new Type[] { typeof(UnityUIPlayerControllerElementGlyph) });
            if (go.GetComponent<UnityEngine.RectTransform>() == null) go.AddComponent<UnityEngine.RectTransform>();
            var layoutGroup = go.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>();
            layoutGroup.spacing = 5;
#if UNITY_2017_PLUS
            layoutGroup.childControlHeight = true;
            layoutGroup.childControlWidth = true;
#endif
            layoutGroup.childForceExpandHeight = true;
            layoutGroup.childForceExpandWidth = true;
            go.GetComponent<UnityEngine.RectTransform>().sizeDelta = new UnityEngine.Vector2(100, 100);
            if (parent != null) go.transform.SetParent(parent, false);
            if (go != null) UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        }
    }
}
