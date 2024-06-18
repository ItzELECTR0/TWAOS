using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(TEnablerValueCommon), true)]
    public class TEnablerValueCommonDrawer : PropertyDrawer
    {
        private const string USS_PATH = EditorPaths.COMMON + "Utilities/Helpers/Enablers/EnablerValueCommon";

        private const string NAME_ROOT = "GC-Enabler-Root";

        public const string PROP_IS_ENABLED = "m_IsEnabled";
        public const string PROP_VALUE = "m_Value";
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement { name = NAME_ROOT };
            
            StyleSheet[] styleSheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in styleSheets) root.styleSheets.Add(styleSheet);

            SerializedProperty isEnabled = property.FindPropertyRelative(PROP_IS_ENABLED);
            SerializedProperty value = property.FindPropertyRelative(PROP_VALUE);

            Label label = new Label(property.displayName);
            
            Toggle toggleIsEnabled = new Toggle(string.Empty)
            {
                bindingPath = isEnabled.propertyPath
            };

            PropertyField fieldValue = new PropertyField(value, string.Empty);
            
            root.Add(label);
            root.Add(toggleIsEnabled);
            root.Add(fieldValue);

            AlignLabel.On(root);

            toggleIsEnabled.RegisterValueChangedCallback(changeEvent =>
            {
                fieldValue.style.display = changeEvent.newValue
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            });
            
            fieldValue.style.display = isEnabled.boolValue
                ? DisplayStyle.Flex
                : DisplayStyle.None;
            
            return root;
        }
    }
}