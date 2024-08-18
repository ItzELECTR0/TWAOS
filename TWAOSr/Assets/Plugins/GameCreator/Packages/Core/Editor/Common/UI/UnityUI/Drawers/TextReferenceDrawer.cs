using GameCreator.Runtime.Common;
using GameCreator.Runtime.Common.UnityUI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common.UnityUI
{
    [CustomPropertyDrawer(typeof(TextReference))]
    public class TextReferenceDrawer : PropertyDrawer
    {
        private const string PROP_TYPE = "m_Type";
        private const string PROP_TEXT = "m_Text";
        private const string PROP_TMP = "m_TMP";

        private const string USS_PATH = EditorPaths.COMMON + "UI/UnityUI/StyleSheets/TextReference";
        
        private const string NAME_ROOT = "GC-UnityUI-TextReference";
        private const string NAME_SELECTOR = "GC-UnityUI-TextReference-Selector";
        private const string NAME_CONTENT = "GC-UnityUI-TextReference-Content";
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement { name = NAME_ROOT };
            
            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet sheet in sheets) root.styleSheets.Add(sheet);

            SerializedProperty propertyType = property.FindPropertyRelative(PROP_TYPE);
            Label fieldLabel = new Label(property.displayName);

            PropertyField fieldSelector = new PropertyField(propertyType, string.Empty)
            {
                name = NAME_SELECTOR
            };

            VisualElement content = new VisualElement
            {
                name = NAME_CONTENT
            };

            fieldSelector.RegisterValueChangeCallback(_ =>
            {
                Refresh(property, content);
            });
            
            Refresh(property, content);
            
            root.Add(fieldLabel);
            root.Add(fieldSelector);
            root.Add(content);

            AlignLabel.On(root);

            return root;
        }

        private static void Refresh(SerializedProperty property, VisualElement content)
        {
            property.serializedObject.Update();
            int type = property.FindPropertyRelative(PROP_TYPE).intValue;
            
            content.Clear();
            
            switch (type)
            {
                case 0: // TextReference.Type.Text
                    SerializedProperty propertyText = property.FindPropertyRelative(PROP_TEXT);
                    PropertyField fieldText = new PropertyField(propertyText, string.Empty);
                    content.Add(fieldText);
                    fieldText.Bind(property.serializedObject);
                    break;
                    
                case 1: // TextReference.Type.TMP
                    SerializedProperty propertyTMP = property.FindPropertyRelative(PROP_TMP);
                    PropertyField fieldTMP = new PropertyField(propertyTMP, string.Empty);
                    content.Add(fieldTMP);
                    fieldTMP.Bind(property.serializedObject);
                    break;
            }
        }
    }
}