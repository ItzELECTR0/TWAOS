using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Variables
{
    [CustomEditor(typeof(LocalNameVariables))]
    public class LocalNameVariablesEditor : TLocalVariablesEditor
    {
        // CREATION MENU: -------------------------------------------------------------------------

        [MenuItem("GameObject/Game Creator/Variables/Name Variables", false, 0)]
        public static void CreateLocalNameVariables(MenuCommand menuCommand)
        {
            GameObject instance = new GameObject("Name Variables");
            instance.AddComponent<LocalNameVariables>();

            GameObjectUtility.SetParentAndAlign(instance, menuCommand?.context as GameObject);

            Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");
            Selection.activeObject = instance;
        }
    }
}
