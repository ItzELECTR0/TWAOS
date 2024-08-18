using GameCreator.Editor.Common;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    [CustomPropertyDrawer(typeof(CollectorListVariable))]
    public class CollectorListVariableDrawer : PropertyDrawer
    {
        private const string PROP_LIST_VARIABLE = "m_ListVariable";
        private const string PROP_LOCAL_LIST = "m_LocalList";
        private const string PROP_GLOBAL_LIST = "m_GlobalList";
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            VisualElement head = new VisualElement();
            VisualElement body = new VisualElement();
            
            root.Add(head);
            root.Add(body);

            SerializedProperty listVariable = property.FindPropertyRelative(PROP_LIST_VARIABLE);
            PropertyField fieldListVariable = new PropertyField(listVariable, property.displayName);

            head.Add(fieldListVariable);
            
            fieldListVariable.RegisterValueChangeCallback(_ =>
            {
                this.UpdateBody(body, property);
            });

            this.UpdateBody(body, property);
            return root;
        }

        private void UpdateBody(VisualElement body, SerializedProperty property)
        {
            SerializedProperty listVariable = property.FindPropertyRelative(PROP_LIST_VARIABLE);
            body.Clear();
            
            switch (listVariable.enumValueIndex)
            {
                case 0 : // Local List Variable
                    SerializedProperty localList = property.FindPropertyRelative(PROP_LOCAL_LIST);
                    PropertyField fieldLocalList = new PropertyField(localList, " ");
                    fieldLocalList.Bind(property.serializedObject);
                    body.Add(fieldLocalList);
                    break;
                    
                case 1 : // Global List Variable
                    SerializedProperty globalList = property.FindPropertyRelative(PROP_GLOBAL_LIST);
                    PropertyField fieldGlobalList = new PropertyField(globalList, " ");
                    fieldGlobalList.Bind(property.serializedObject);
                    body.Add(fieldGlobalList);
                    break;
            }
        }
    }
}