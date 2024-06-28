using UnityEngine;
using UnityEditor;

#if UNITY_STANDALONE_WIN && UNITY_64
using UnityEngine.NVIDIA;
#endif
#if DLSS_INSTALLED
using NVIDIA = UnityEngine.NVIDIA;
#endif


namespace AEG.DLSS
{
    [CustomEditor(typeof(DLSS_UTILS), editorForChildClasses: true)]
    public class DLSS_Editor : Editor
    {
        public override void OnInspectorGUI() {
#if !DLSS_INSTALLED
#if UNITY_URP
            EditorGUILayout.LabelField("----- Missing NVIDIA DLSS Package ------", EditorStyles.boldLabel);
            if(GUILayout.Button("Install Package")) {
                UnityEditor.PackageManager.Client.Add("com.unity.modules.nvidia");
                PipelineDefines.AddDefine("DLSS_INSTALLED");
                AssetDatabase.Refresh();
            }
#endif

#elif AEG_DLSS

#if !UNITY_STANDALONE_WIN || !UNITY_64
            EditorGUILayout.LabelField("----- DLSS is not supported on this platform ------", EditorStyles.boldLabel);
#endif
            DLSS_UTILS dlssScript = target as DLSS_UTILS;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("DLSS Settings", EditorStyles.boldLabel);
            DLSS_Quality dlssQuality = (DLSS_Quality)EditorGUILayout.EnumPopup(Styles.qualityText, dlssScript.DLSSQuality);

            float antiGhosting = dlssScript.m_antiGhosting;
            antiGhosting = EditorGUILayout.Slider(Styles.antiGhostingText, dlssScript.m_antiGhosting, 0.0f, 1.0f);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if(EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(dlssScript);

                Undo.RecordObject(target, "Changed Area Of Effect");
                dlssScript.DLSSQuality = dlssQuality;
                dlssScript.m_antiGhosting = antiGhosting;
            }
#endif
        }

        private static class Styles
        {
            public static GUIContent qualityText = new GUIContent("Quality", "Quality 1.5, Balanced 1.7, Performance 2, Ultra Performance 3");
            public static GUIContent antiGhostingText = new GUIContent("Anti-Ghosting", "The Anti-Ghosting value between 0 and 1, where 0 is no additional anti-ghosting and 1 is maximum additional Anti-Ghosting.");
        }
    }
}
