using UnityEditor;
using UnityEngine.UIElements;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(Save))]
    public class SaveDrawer : PropertyDrawer
    {
        private const string PATH_STYLES = EditorPaths.COMMON + "Structures/Save/StyleSheets/Save";
        
        private static readonly IIcon ICON_ON = new IconDiskSolid(ColorTheme.Type.Green);
        private static readonly IIcon ICON_OFF = new IconDiskOutline(ColorTheme.Type.TextLight);

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty propertySave = property.FindPropertyRelative("m_Save");

            VisualElement root = new VisualElement();
            
            StyleSheet[] styleSheets = StyleSheetUtils.Load(PATH_STYLES);
            foreach (StyleSheet styleSheet in styleSheets) root.styleSheets.Add(styleSheet);
            
            Button button = new Button
            {
                tooltip = "Allow to save the value between play sessions"
            };

            Image image = new Image { focusable = false };
            Label label = new Label { focusable = false };

            button.clicked += () =>
            {
                property.serializedObject.Update();
                propertySave.boolValue = !propertySave.boolValue;

                SerializationUtils.ApplyUnregisteredSerialization(property.serializedObject);
                this.Update(label, image, propertySave.boolValue);
            };

            this.Update(label, image, propertySave.boolValue);
            
            root.AddToClassList("gc-save-root");
            button.AddToClassList("gc-save-btn");
            image.AddToClassList("gc-save-image");
            label.AddToClassList("gc-save-label");

            button.Add(image);
            button.Add(label);

            root.Add(button);
            return root;
        }

        private void Update(Label label, Image image, bool state)
        {
            label.text = state ? "ON" : "OFF";
            image.image = state ? ICON_ON.Texture : ICON_OFF.Texture;
        }
    }
}