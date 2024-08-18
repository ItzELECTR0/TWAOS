using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    [CustomPropertyDrawer(typeof(FieldGetGlobalName))]
    public class FieldGetGlobalNameDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            SerializedProperty variable = property.FindPropertyRelative("m_Variable");
            SerializedProperty typeID = property.FindPropertyRelative("m_TypeID");

            ObjectField fieldVariable = new ObjectField(variable.displayName)
            {
                allowSceneObjects = false,
                objectType = typeof(GlobalNameVariables),
                bindingPath = variable.propertyPath
            };
            
            fieldVariable.AddToClassList(AlignLabel.CLASS_UNITY_ALIGN_LABEL);

            SerializedProperty typeIDStr = typeID.FindPropertyRelative(IdStringDrawer.NAME_STRING);
            IdString typeIDValue = new IdString(typeIDStr.stringValue);
            
            GlobalNamePickTool toolPickName = new GlobalNamePickTool(
                property,
                typeIDValue,
                true
            );

            fieldVariable.RegisterValueChangedCallback(_ => toolPickName.OnChangeAsset());
            
            root.Add(fieldVariable);
            root.Add(toolPickName);

            return root;
        }
    }
}