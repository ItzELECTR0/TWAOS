#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Heat
{
    public class InitHeatUI
    {
        [InitializeOnLoad]
        public class InitOnLoad
        {
            static InitOnLoad()
            {
                if (!EditorPrefs.HasKey("HeatUI.Installed"))
                {
                    EditorPrefs.SetInt("HeatUI.Installed", 1);
                    EditorUtility.DisplayDialog("Hello there!", "Thank you for purchasing Heat UI." +
                        "\n\nTo use the UI Manager, go to Tools > Heat UI > Show UI Manager." +
                        "\n\nIf you need help, feel free to contact us through our support channels.", "Got it!");
                }

                if (!EditorPrefs.HasKey("HeatUI.HasCustomEditorData"))
                {
                    string darkPath = AssetDatabase.GetAssetPath(Resources.Load("HeatUIEditor-Dark"));
                    string lightPath = AssetDatabase.GetAssetPath(Resources.Load("HeatUIEditor-Light"));

                    EditorPrefs.SetString("HeatUI.CustomEditorDark", darkPath);
                    EditorPrefs.SetString("HeatUI.CustomEditorLight", lightPath);
                    EditorPrefs.SetInt("HeatUI.HasCustomEditorData", 1);
                }
            }
        }
    }
}
#endif