using Unity.Multiplayer.Editor;
using UnityEditor;
using UnityEditor.Build.Profile;
using Unity.DedicatedServer.Editor.Internal;

namespace Unity.DedicatedServer
{
    internal class CLISettingsSection : IMultiplayerBuildOptionsSection
    {
        public int Order => 100;

        private bool m_Expanded = false;

        public void DrawBuildOptions(BuildProfile profile)
        {
            if (!InternalUtility.IsServerProfile(profile))
                return;

            m_Expanded = EditorGUILayout.Foldout(m_Expanded, "CLI Arguments defaults");

            if (m_Expanded)
            {
                EditorGUI.indentLevel++;
                CLIDefaults.OnGUI();
                EditorGUI.indentLevel--;
            }
        }
    }
}
