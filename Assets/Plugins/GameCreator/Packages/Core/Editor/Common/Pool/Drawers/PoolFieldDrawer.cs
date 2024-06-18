using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(PoolField))]
    public class PoolFieldDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            StyleSheet[] sheets = StyleSheetUtils.Load();
            foreach (StyleSheet sheet in sheets) root.styleSheets.Add(sheet);

            SerializedProperty prefab = property.FindPropertyRelative("m_Prefab");
            SerializedProperty usePooling = property.FindPropertyRelative("m_UsePooling");
            SerializedProperty duration = property.FindPropertyRelative("m_Duration");

            PropertyField fieldUsePooling = new PropertyField(usePooling);
            PropertyField fieldDuration = new PropertyField(duration);
            
            root.Add(new PropertyField(prefab));
            root.Add(fieldUsePooling);
            root.Add(fieldDuration);
            
            fieldUsePooling.RegisterValueChangeCallback(changeEvent =>
            {
                SerializedProperty change = changeEvent.changedProperty;
                bool isEnabled = change.FindPropertyRelative("m_IsEnabled").boolValue;
                
                fieldDuration.style.display = isEnabled
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            });
            
            bool isEnabled = usePooling.FindPropertyRelative("m_IsEnabled").boolValue;
            fieldDuration.style.display = isEnabled
                ? DisplayStyle.Flex
                : DisplayStyle.None;
            
            return root;
        }
    }
}