using UnityEditor;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.VisualScripting
{
    [CustomEditor(typeof(Actions))]
    public class ActionsEditor : BaseActionsEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement container = new VisualElement();
            container.style.marginTop = DEFAULT_MARGIN_TOP;

            this.CreateInstructionsGUI(container);

            return container;
        }
        
        // CREATION MENU: -------------------------------------------------------------------------
        
        [MenuItem("GameObject/Game Creator/Visual Scripting/Actions", false, 0)]
        public static void CreateElement(MenuCommand menuCommand)
        {
            GameObject instance = new GameObject("Actions");
            instance.AddComponent<Actions>();
            
            GameObjectUtility.SetParentAndAlign(instance, menuCommand?.context as GameObject);

            Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");
            Selection.activeObject = instance;
        }
    }
}