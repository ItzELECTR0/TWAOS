// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

namespace Rewired.Glyphs.Editor {

    public static class EditorMenu {

        private const string logPrefix = "Rewired: ";
        private const string helpMenuCreate = "Window/Rewired/Create/Glyphs/";
        private const string assetsCreate = "Assets/Create/Rewired/Glyphs/";
        private const string componentMenu = "Component/Rewired/Glyphs/";

        [UnityEditor.MenuItem(helpMenuCreate + "Sprite Glyph Set")]
        [UnityEditor.MenuItem(assetsCreate + "Sprite Glyph Set")]
        public static void CreateSpriteGlyphSet() {
            CreateScriptableObjectAsset<SpriteGlyphSet>(false);
        }

        [UnityEditor.MenuItem(helpMenuCreate + "Glyph Set Collection")]
        [UnityEditor.MenuItem(assetsCreate + "Glyph Set Collection")]
        public static void CreateGlyphSetCollection() {
            CreateScriptableObjectAsset<GlyphSetCollection>(false);
        }

        [UnityEditor.MenuItem(helpMenuCreate + "Controller Element Glyph Selector Options")]
        [UnityEditor.MenuItem(assetsCreate + "Controller Element Glyph Selector Options")]
        public static void CreateControllerElementGlyphSelectorOptions() {
            Rewired.Glyphs.Editor.EditorMenu.CreateScriptableObjectAsset<ControllerElementGlyphSelectorOptionsSO>(false);
        }

        [UnityEditor.MenuItem(helpMenuCreate + "Glyph Provider")]
        [UnityEditor.MenuItem(componentMenu + "Glyph Provider")]
        public static void CreateGlyphProvider() {
            
            UnityEngine.GameObject go = UnityEditor.Selection.activeGameObject;
            if (go == null) {
                UnityEngine.Debug.LogError(logPrefix + "You must select a GameObject.");
                return;
            }
            if (UnityEditor.Selection.gameObjects.Length > 1) {
                UnityEngine.Debug.LogError(logPrefix + "You must select only one GameObject.");
                return;
            }

            GlyphProvider component = UnityEditor.Undo.AddComponent<GlyphProvider>(go);

            string glyphCollectionPath = UnityEditor.AssetDatabase.GUIDToAssetPath("9489dc740d1e9c340b1acc6947c899c6");
            UnityEngine.Object collection;
            if (string.IsNullOrEmpty(glyphCollectionPath) ||
                (collection = UnityEditor.AssetDatabase.LoadAssetAtPath(glyphCollectionPath, typeof(GlyphSetCollection))) == null) {
                UnityEngine.Debug.LogError(logPrefix + "Default Controller Element Glyph Set Collection is missing.");
                return;
            }

            UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(component);
            so.Update();
            UnityEditor.SerializedProperty sp = so.FindProperty("_glyphSetCollections");
            sp.InsertArrayElementAtIndex(0);
            sp.GetArrayElementAtIndex(0).objectReferenceValue = collection;
            so.ApplyModifiedProperties();
        }

        public static string CreateScriptableObjectAsset<T>(bool saveNow) where T : UnityEngine.ScriptableObject {
            T asset = UnityEngine.ScriptableObject.CreateInstance<T>();

            string path = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject);
            if (path == "") {
                path = "Assets";
            } else if (System.IO.Path.GetExtension(path) != "") {
                path = path.Replace(System.IO.Path.GetFileName(UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject)), "");
            }

            string assetPathAndName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).Name.ToString() + ".asset");

            UnityEditor.AssetDatabase.CreateAsset(asset, assetPathAndName);

            if (saveNow) UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = asset;

            return assetPathAndName;
        }
    }
}
