using GameCreator.Editor.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;

namespace GameCreator.Editor.VisualScripting
{
    [CustomEditor(typeof(Conditions))]
    public class ConditionsEditor : UnityEditor.Editor
    {
        protected static readonly StyleLength DEFAULT_MARGIN_TOP = new StyleLength(5);
        
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement container = new VisualElement();
            container.style.marginTop = DEFAULT_MARGIN_TOP;
            
            SerializedProperty branches = this.serializedObject.FindProperty("m_Branches");

            PropertyField fieldBranches = new PropertyField(branches);
            container.Add(fieldBranches);

            return container;
        }
        
        // CREATION MENU: -------------------------------------------------------------------------
        
        [MenuItem("GameObject/Game Creator/Visual Scripting/Conditions", false, 0)]
        public static void CreateElement(MenuCommand menuCommand)
        {
            GameObject instance = new GameObject("Conditions");
            instance.AddComponent<Conditions>();
            
            GameObjectUtility.SetParentAndAlign(instance, menuCommand?.context as GameObject);

            Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");
            Selection.activeObject = instance;
        }
    }
}