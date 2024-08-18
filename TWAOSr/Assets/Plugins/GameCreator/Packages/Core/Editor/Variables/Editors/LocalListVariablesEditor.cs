using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Variables
{
    [CustomEditor(typeof(LocalListVariables))]
    public class LocalListVariablesEditor : TLocalVariablesEditor
    {
        // CREATION MENU: -------------------------------------------------------------------------

        [MenuItem("GameObject/Game Creator/Variables/List Variables", false, 0)]
        public static void CreateLocalListVariables(MenuCommand menuCommand)
        {
            GameObject instance = new GameObject("List Variables");
            instance.AddComponent<LocalListVariables>();

            GameObjectUtility.SetParentAndAlign(instance, menuCommand?.context as GameObject);

            Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");
            Selection.activeObject = instance;
        }
    }
}
