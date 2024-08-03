using UnityEngine;
using UnityEditor;

#if UNITY_STANDALONE_WIN && UNITY_64
using UnityEngine.NVIDIA;
#endif
#if DLSS_INSTALLED
using NVIDIA = UnityEngine.NVIDIA;
#endif


namespace TND.DLSS
{
    [CustomEditor(typeof(DLSS_UTILS), editorForChildClasses: true)]
    public class DLSS_Editor : Editor
    {
        public override void OnInspectorGUI() {
#if !DLSS_INSTALLED
#if UNITY_URP || UNITY_HDRP
            EditorGUILayout.LabelField("----- Missing NVIDIA DLSS Package ------", EditorStyles.boldLabel);
            if(GUILayout.Button("Install Package")) {
                UnityEditor.PackageManager.Client.Add("com.unity.modules.nvidia");
                PipelineDefines.AddDefine("DLSS_INSTALLED");
                AssetDatabase.Refresh();
            }
#endif

#elif TND_DLSS

#if !UNITY_STANDALONE_WIN || !UNITY_64
            EditorGUILayout.LabelField("----- DLSS is not supported on this platform ------", EditorStyles.boldLabel);
#endif
            DLSS_UTILS dlssScript = target as DLSS_UTILS;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("DLSS Settings", EditorStyles.boldLabel);
            DLSS_Quality dlssQuality = (DLSS_Quality)EditorGUILayout.EnumPopup(Styles.qualityText, dlssScript.DLSSQuality);

            float antiGhosting = dlssScript.m_antiGhosting;
            antiGhosting = EditorGUILayout.Slider(Styles.antiGhostingText, dlssScript.m_antiGhosting, 0.0f, 1.0f);

            bool sharpening = EditorGUILayout.Toggle(Styles.sharpeningText, dlssScript.sharpening);
            float sharpness = dlssScript.sharpness;
            if (dlssScript.sharpening)
            {
                EditorGUI.indentLevel++;
                sharpness = EditorGUILayout.Slider(Styles.sharpnessText, dlssScript.sharpness, 0.0f, 1.0f);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if(EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(dlssScript);

                Undo.RecordObject(target, "Changed Area Of Effect");
                dlssScript.DLSSQuality = dlssQuality;
                dlssScript.m_antiGhosting = antiGhosting;
                dlssScript.sharpening = sharpening;
                dlssScript.sharpness = sharpness;
            }
#endif
        }

        private static class Styles
        {
            public static GUIContent qualityText = new GUIContent("Quality", "Quality 1.5, Balanced 1.7, Performance 2, Ultra Performance 3");
            public static GUIContent antiGhostingText = new GUIContent("Anti-Ghosting", "The Anti-Ghosting value between 0 and 1, where 0 is no additional anti-ghosting and 1 is maximum additional Anti-Ghosting.");
            public static GUIContent sharpeningText = new GUIContent("Sharpening", "Enable an additional sharpening in the dlss algorithm.");
            public static GUIContent sharpnessText = new GUIContent("Sharpness", "The sharpness value between 0 and 1, where 0 is no additional sharpness and 1 is maximum additional sharpness.");

        }
    }
}
