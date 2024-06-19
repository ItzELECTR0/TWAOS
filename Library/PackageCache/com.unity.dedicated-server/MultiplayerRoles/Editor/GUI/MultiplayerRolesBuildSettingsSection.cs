using UnityEditor;
using UnityEditor.Build.Profile;

namespace Unity.Multiplayer.Editor
{
    internal class MultiplayerRoleBuildSettingsSection : IMultiplayerBuildOptionsSection
    {
        public int Order => 100;

        public void DrawBuildOptions(BuildProfile profile)
        {
            if (!EditorMultiplayerRolesManager.EnableMultiplayerRoles)
                return;

            EditorGUI.BeginChangeCheck();
            var target = (MultiplayerRoleFlags)EditorGUILayout.EnumPopup(
                "Multiplayer Role",
                MultiplayerRolesSettings.instance.GetMultiplayerRoleForBuildProfile(profile));

            if (EditorGUI.EndChangeCheck())
                MultiplayerRolesSettings.instance.SetMultiplayerRoleForBuildProfile(profile, target);
        }
    }
}
