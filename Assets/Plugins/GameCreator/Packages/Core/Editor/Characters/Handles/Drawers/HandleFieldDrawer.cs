using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(HandleField))]
    public class HandleFieldDrawer : PropertyDrawer
    {
        private const string CLASS_INDENT = "gc-indent";
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            VisualElement head = new VisualElement();
            VisualElement contentValue = new VisualElement();
            VisualElement contentHandle = new VisualElement();
            
            StyleSheet[] styleSheets = StyleSheetUtils.Load();
            foreach (StyleSheet styleSheet in styleSheets) root.styleSheets.Add(styleSheet);

            root.Add(head);
            root.Add(contentValue);
            root.Add(contentHandle);
            
            contentValue.AddToClassList(CLASS_INDENT);
            contentHandle.AddToClassList(CLASS_INDENT);

            SerializedProperty type = property.FindPropertyRelative("m_Type");
            SerializedProperty bone = property.FindPropertyRelative("m_Bone");
            SerializedProperty position = property.FindPropertyRelative("m_LocalPosition");
            SerializedProperty rotation = property.FindPropertyRelative("m_LocalRotation");
            SerializedProperty handle = property.FindPropertyRelative("m_Handle");
            
            PropertyField fieldType = new PropertyField(type);
            head.Add(fieldType);
            
            PropertyField fieldBone = new PropertyField(bone);
            PropertyField fieldPosition = new PropertyField(position);
            PropertyField fieldRotation = new PropertyField(rotation);
            
            contentValue.Add(fieldBone);
            contentValue.Add(fieldPosition);
            contentValue.Add(fieldRotation);
            
            PropertyField fieldHandle = new PropertyField(handle, " ");
            contentHandle.Add(fieldHandle);

            bool isValue = type.enumValueIndex == 0;
            contentValue.style.display = isValue ? DisplayStyle.Flex : DisplayStyle.None;
            contentHandle.style.display = isValue ? DisplayStyle.None : DisplayStyle.Flex;
            
            fieldType.RegisterValueChangeCallback(changeEvent =>
            {
                bool newIsValue = changeEvent.changedProperty.enumValueIndex == 0;
                contentValue.style.display = newIsValue ? DisplayStyle.Flex : DisplayStyle.None;
                contentHandle.style.display = newIsValue ? DisplayStyle.None : DisplayStyle.Flex;
            });
            
            return root;
        }
        
        
    }
}