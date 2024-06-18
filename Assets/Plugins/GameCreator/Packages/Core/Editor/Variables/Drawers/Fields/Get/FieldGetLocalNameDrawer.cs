using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    [CustomPropertyDrawer(typeof(FieldGetLocalName))]
    public class FieldGetLocalNameDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            SerializedProperty variable = property.FindPropertyRelative("m_Variable");
            SerializedProperty typeID = property.FindPropertyRelative("m_TypeID");

            PropertyField fieldVariable = new PropertyField(variable, variable.displayName);
            
            SerializedProperty typeIDStr = typeID.FindPropertyRelative(IdStringDrawer.NAME_STRING);
            IdString typeIDValue = new IdString(typeIDStr.stringValue);
            
            LocalNamePickTool toolPickName = new LocalNamePickTool(
                property,
                typeIDValue,
                true
            );
            
            fieldVariable.RegisterValueChangeCallback(_ => toolPickName.OnChangeAsset());

            root.Add(fieldVariable);
            root.Add(toolPickName);

            return root;
        }
    }
}