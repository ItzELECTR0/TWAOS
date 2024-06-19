using Unity.DedicatedServer.Editor.Internal;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Unity.Multiplayer.Editor
{
    internal class MultiplayerRolesBuildPreprocessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        private static MultiplayerRoleFlags? s_PreviousRole = null;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!EditorMultiplayerRolesManager.EnableMultiplayerRoles)
                return;

            s_PreviousRole = EditorMultiplayerRolesManager.ActiveMultiplayerRoleMask;

            var profile = InternalUtility.GetActiveOrClassicProfile();
            var multiplayerRole = EditorMultiplayerRolesManager.GetMultiplayerRoleForBuildProfile(profile);
            EditorMultiplayerRolesManager.ActiveMultiplayerRoleMask = multiplayerRole;

            UnityEngine.Debug.Log($"Building with multiplayer role: {multiplayerRole}");

            EditorApplication.delayCall += RevertToPreviousRole;
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            if (!EditorMultiplayerRolesManager.EnableMultiplayerRoles)
                return;

            RevertToPreviousRole();
        }

        private static void RevertToPreviousRole()
        {
            if (!s_PreviousRole.HasValue)
                return;

            EditorMultiplayerRolesManager.ActiveMultiplayerRoleMask = s_PreviousRole.Value;
            s_PreviousRole = null;
        }
    }
}
