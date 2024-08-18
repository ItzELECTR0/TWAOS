using GameCreator.Editor.Common;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    [CustomPropertyDrawer(typeof(NameVariableRuntime))]
    public class NameVariablesRuntimeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return CreateGUI(property);
        }
        
        // PAINT METHOD: --------------------------------------------------------------------------

        public static VisualElement CreateGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            SerializedProperty list = property.FindPropertyRelative("m_List");
            NameVariableRuntime runtime = property.GetValue<NameVariableRuntime>();

            Object target = property.serializedObject.targetObject;

            bool isPlaymode = EditorApplication.isPlayingOrWillChangePlaymode;
            bool isAsset = PrefabUtility.IsPartOfPrefabAsset(target) ||
                           target is ScriptableObject;

            VisualElement tool = isPlaymode && !isAsset
                ? new NameListView(runtime)
                : new NameListTool(list);

            root.Add(tool);
            return root;
        }
    }
}