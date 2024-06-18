using UnityEditor;

namespace GameCreator.Editor.Installs
{
    public static class UninstallCore
    {
        [MenuItem(
            itemName: "Game Creator/Uninstall/Game Creator",
            isValidateFunction: false,
            priority: UninstallManager.PRIORITY
        )]
        
        private static void Uninstall()
        {
            UninstallManager.Uninstall("Core");
        }
    }
}