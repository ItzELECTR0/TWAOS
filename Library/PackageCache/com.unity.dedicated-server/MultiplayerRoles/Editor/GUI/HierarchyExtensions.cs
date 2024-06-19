using UnityEditor;
using UnityEngine;
using InternalManager = UnityEditor.Multiplayer.Internal.EditorMultiplayerManager;

namespace Unity.Multiplayer.Editor
{
    internal static class HierarchyExtensions
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnItemGUI;
            InternalManager.enableMultiplayerRolesChanged += OnMultiplayerRolesStateChange;
            InternalManager.activeMultiplayerRoleChanged += OnMultiplayerRolesStateChange;
        }

        private static void OnMultiplayerRolesStateChange()
        {
            EditorApplication.RepaintHierarchyWindow();
        }

        private static void OnItemGUI(int instanceId, Rect selectionRect)
        {
            if (!InternalManager.enableMultiplayerRoles)
                return;

            var gameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;

            if (gameObject == null)
                return;

            var target = EditorMultiplayerRolesManager.GetMultiplayerRoleMaskForGameObject(gameObject);

            if (target == MultiplayerRoleFlags.ClientAndServer)
                return;

            var iconName = target == MultiplayerRoleFlags.Server
                ? "BuildSettings.DedicatedServer On"
                : (target == MultiplayerRoleFlags.Client ? "BuildSettings.Standalone On" : null);


            var activeTarget = EditorMultiplayerRolesManager.ActiveMultiplayerRoleMask;

            var isStripped = (activeTarget & target) == 0;
            var backgroundColor = isStripped ? new Color32(253, 134, 120, 255) : new Color32(196, 196, 196, 255);

            if (isStripped)
            {
                var rect = selectionRect;
                rect.width = 2;
                rect.height -= 2;
                rect.x = 36;
                rect.y += 1;
                // EditorGUI.DrawRect(rect, backgroundColor);
            }

            if (iconName != null)
            {
                var iconRect = selectionRect;
                iconRect.width = iconRect.height = 16;
                iconRect.x = selectionRect.xMax - iconRect.width;

#if UNITY_CINEMACHINE_PRESENT
                if (gameObject.TryGetComponent(out Cinemachine.CinemachineBrain _))
                {
                    iconRect.x -= iconRect.width + 2;
                }
#endif

                // EditorGUI.DrawRect(iconRect, new Color32(31, 31, 31, 255));

                var icon = EditorGUIUtility.IconContent(iconName).image;
                GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit, true, 1, backgroundColor, 0, 0);
            }
        }
    }
}
