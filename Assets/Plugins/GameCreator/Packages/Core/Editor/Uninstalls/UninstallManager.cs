using System;
using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Installs
{
    public static class UninstallManager
    {
        public const int PRIORITY = 102;

        private const string UNINSTALL_TITLE = "Are you sure you want to uninstall {0}";
        private const string UNINSTALL_MSG = "This operation cannot be undone";

        // EVENTS: --------------------------------------------------------------------------------

        public static event Action<string> EventBeforeUninstall;
        public static event Action<string> EventAfterUninstall;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static void Uninstall(string moduleFolder)
        {
            string path = EditorPaths.PACKAGES + moduleFolder;
            if (!AssetDatabase.IsValidFolder(path)) return;

            bool delete = EditorUtility.DisplayDialog(
                string.Format(UNINSTALL_TITLE, moduleFolder),
                UNINSTALL_MSG, 
                "Yes", "Cancel"
            );
            
            if (!delete) return;
            EventBeforeUninstall?.Invoke(moduleFolder);
            
            AssetDatabase.MoveAssetToTrash(path);

            EventAfterUninstall?.Invoke(moduleFolder);
        }
    }
}
