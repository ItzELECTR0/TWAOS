// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649 // disable warnings about unused variables

#if UNITY_EDITOR

namespace Rewired.Demos.CustomPlatform {

    public static class CreateScriptableObjects {

        /// <summary>
        /// Creates a scriptable object asset.
        /// </summary>
        [UnityEditor.MenuItem("Assets/Create/Rewired/Demos/CustomPlatform/MyPlatformHardwareJoystickMapPlatformMap")]
        public static void CreateMyPlatformHardwareJoystickMapPlatformMap() {
            EditorTools.CreateScriptableObjectAsset<MyPlatformHardwareJoystickMapPlatformMap>();
        }

        /// <summary>
        /// Creates a scriptable object asset.
        /// </summary>
        [UnityEditor.MenuItem("Assets/Create/Rewired/Demos/CustomPlatform/CustomPlatformHardwareJoystickMapPlatformDataSet")]
        public static void CreateCustomPlatformHardwareJoystickMapPlatformDataSet() {
            EditorTools.CreateScriptableObjectAsset<CustomPlatformHardwareJoystickMapPlatformDataSet>();
        }
        
        private static class EditorTools {

            private static string ASSET_PATH_REL = "Assets";

            /// <summary>
            //	This makes it easy to create, name and place unique new ScriptableObject asset files.
            /// </summary>
            public static string CreateScriptableObjectAsset<T>(bool saveNow) where T : UnityEngine.ScriptableObject {
                T asset = UnityEngine.ScriptableObject.CreateInstance<T>();

                string path = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject);
                if (path == "") {
                    path = ASSET_PATH_REL;
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
            public static string CreateScriptableObjectAsset<T>() where T : UnityEngine.ScriptableObject {
                return CreateScriptableObjectAsset<T>(true);
            }
        }
    }
}

#endif
