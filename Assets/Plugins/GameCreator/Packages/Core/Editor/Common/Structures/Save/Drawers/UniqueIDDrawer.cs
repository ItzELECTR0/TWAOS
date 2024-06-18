using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(UniqueID))]
    public class UniqueIDDrawer : PropertyDrawer
    {
        private const string PATH_STYLES = EditorPaths.COMMON + "Structures/Save/StyleSheets/UniqueID";

        public const string SERIALIZED_ID = "m_SerializedID";
        
        private static readonly GUIContent TXT_COPY = new GUIContent("Copied to Clipboard");
        private static readonly IIcon ICON_COPY = new IconCopy(ColorTheme.Type.TextLight);

        private const string NAME_HEAD = "GC-UniqueID-Head";
        private const string NAME_BODY = "GC-UniqueID-Body";
        private const string NAME_HEAD_LABEL = "GC-UniqueID-Head-Label";
        private const string NAME_HEAD_BUTTON_EDIT = "GC-UniqueID-Head-BtnEdit";
        private const string NAME_HEAD_BUTTON_COPY = "GC-UniqueID-Head-BtnCopy";
        private const string NAME_BODY_TEXTFIELD_ID = "GC-UniqueID-Body-Property";
        private const string NAME_BODY_BUTTON_REGEN = "GC-UniqueID-Body-BtnRegen";
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            VisualElement head = new VisualElement { name = NAME_HEAD };
            VisualElement body = new VisualElement { name = NAME_BODY };

            root.Add(head);
            root.Add(body);

            StyleSheet[] sheets = StyleSheetUtils.Load(PATH_STYLES);
            foreach (StyleSheet sheet in sheets) root.styleSheets.Add(sheet);

            property.isExpanded = false;
            
            SerializedProperty id = property.FindPropertyRelative(SERIALIZED_ID);
            SerializedProperty idString = id.FindPropertyRelative(IdStringDrawer.NAME_STRING);

            Label labelHead = new Label
            {
                text = "ID",
                name = NAME_HEAD_LABEL
            };
            
            Button buttonHeadEdit = new Button
            {
                text = idString.stringValue,
                name = NAME_HEAD_BUTTON_EDIT
            };

            Button buttonHeadCopy = new Button
            {
                text = string.Empty,
                name = NAME_HEAD_BUTTON_COPY
            };

            Image imageHeadCopy = new Image { image = ICON_COPY.Texture };
            buttonHeadCopy.Add(imageHeadCopy);

            buttonHeadEdit.clicked += () =>
            {
                property.isExpanded = !property.isExpanded;
                this.UpdateEditState(property.isExpanded, body);
            };
            
            buttonHeadCopy.clicked += () =>
            {
                GUIUtility.systemCopyBuffer = idString.stringValue;
                if (EditorWindow.focusedWindow != null)
                {
                    EditorWindow.focusedWindow.ShowNotification(TXT_COPY);
                }
            };
            
            head.Add(labelHead);
            head.Add(buttonHeadEdit);
            head.Add(buttonHeadCopy);

            PropertyField fieldEditProperty = new PropertyField(idString, string.Empty)
            {
                name = NAME_BODY_TEXTFIELD_ID
            };

            Button buttonEditRegen = new Button
            {
                text = "Regenerate",
                name = NAME_BODY_BUTTON_REGEN
            };

            fieldEditProperty.RegisterValueChangeCallback(_ =>
            {
                buttonHeadEdit.text = idString.stringValue;
            });

            buttonEditRegen.clicked += () =>
            {
                idString.stringValue = UniqueID.GenerateID();
                id.serializedObject.ApplyModifiedProperties();
                id.serializedObject.Update();
            };
            
            body.Add(fieldEditProperty);
            body.Add(buttonEditRegen);

            this.UpdateEditState(property.isExpanded, body);

            return root;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void UpdateEditState(bool state, VisualElement body)
        {
            body.style.display = state
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }
    }
}