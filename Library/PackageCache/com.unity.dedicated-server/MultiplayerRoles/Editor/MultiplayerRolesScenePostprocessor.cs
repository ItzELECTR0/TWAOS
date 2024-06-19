using Unity.Profiling;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;

namespace Unity.Multiplayer.Editor
{
    internal class MuiltiplayerRolesScenePostprocessor : IProcessSceneWithReport
    {
        public int callbackOrder => 0;

        private static readonly ProfilerMarker s_OnProcessSceneMarker = new ProfilerMarker("MultiplayerRolesScenePostprocessor.OnProcessScene");
        public void OnProcessScene(Scene scene, BuildReport report)
        {
            using var marker = s_OnProcessSceneMarker.Auto();

            if (!EditorMultiplayerRolesManager.EnableMultiplayerRoles || !EditorMultiplayerRolesManager.EnableSafetyChecks)
                return;

            var referenceTracker = new ReferenceTracker(EditorMultiplayerRolesManager.ActiveMultiplayerRoleMask, scene);
            referenceTracker.Collect();
            referenceTracker.WarnBrokenReferencesIfNeeded();
        }
    }
}
