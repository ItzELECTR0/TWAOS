using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(PropertyGetInstantiate), true)]
    public class PropertyGetInstantiateDrawer : PropertyDrawer
    {
        private const string CLASS_INLINE_FIELD = "gc-inline-toggle-field";
        
        private const string USE_POOLING = "usePooling";
        private const string SIZE = "size";
        private const string HAS_DURATION = "hasDuration";
        private const string DURATION = "duration";
        
        // PAINT METHOD: --------------------------------------------------------------------------
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            StyleSheet[] sheets = StyleSheetUtils.Load();
            foreach (StyleSheet sheet in sheets) root.styleSheets.Add(sheet);

            SerializedProperty item = property.FindPropertyRelative(IPropertyDrawer.PROPERTY_NAME);

            SerializationUtils.CreateChildProperties(
                root, property, 
                item.HideLabelsInEditor() 
                    ? SerializationUtils.ChildrenMode.HideLabelsInChildren 
                    : SerializationUtils.ChildrenMode.ShowLabelsInChildren,
                true,
                IPropertyDrawer.PROPERTY_NAME,
                USE_POOLING, SIZE, HAS_DURATION, DURATION
            );
            
            root.Add(new PropertyElement(
                item,
                property.displayName, 
                false
            ));
            
            SerializedProperty usePooling = property.FindPropertyRelative(USE_POOLING);
            SerializedProperty size = property.FindPropertyRelative(SIZE);
            SerializedProperty hasDuration = property.FindPropertyRelative(HAS_DURATION);
            SerializedProperty duration = property.FindPropertyRelative(DURATION);

            Toggle fieldUsePooling = new Toggle
            {
                label = usePooling.displayName,
                bindingPath = usePooling.propertyPath
            };
            
            PropertyField fieldSize = new PropertyField(size, string.Empty);
            
            Toggle fieldHasDuration = new Toggle
            {
                label = hasDuration.displayName,
                bindingPath = hasDuration.propertyPath
            };
            
            PropertyField fieldDuration = new PropertyField(duration, string.Empty);

            VisualElement contentPooling = new VisualElement();
            VisualElement contentDuration = new VisualElement();
            
            contentPooling.AddToClassList(CLASS_INLINE_FIELD);
            contentDuration.AddToClassList(CLASS_INLINE_FIELD);
            
            contentPooling.Add(fieldUsePooling);
            contentPooling.Add(fieldSize);
            contentDuration.Add(fieldHasDuration);
            contentDuration.Add(fieldDuration);

            AlignLabel.On(contentPooling);
            AlignLabel.On(contentDuration);
            
            root.Add(contentPooling);
            root.Add(contentDuration);

            fieldUsePooling.RegisterValueChangedCallback(changeEvent =>
            {
                fieldSize.SetEnabled(changeEvent.newValue);
                contentDuration.style.display = changeEvent.newValue
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            });
            
            fieldSize.SetEnabled(usePooling.boolValue);
            contentDuration.style.display = usePooling.boolValue
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            fieldHasDuration.RegisterValueChangedCallback(changeEvent =>
            {
                fieldDuration.SetEnabled(changeEvent.newValue);
            });
            
            fieldDuration.SetEnabled(hasDuration.boolValue);

            return root;
        }
    }
}