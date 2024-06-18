using System.Collections.Generic;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Common.UnityUI;
using UnityEditor;
using UnityEditor.UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common.UnityUI
{
    [CustomEditor(typeof(DropdownPropertyInteger))]
    public class DropdownPropertyIntegerEditor : DropdownEditor
    {
        private const string USS_PATH = EditorPaths.COMMON + "UI/UnityUI/StyleSheets/Dropdown";

        private const string NAME_ROOT = "GC-UI-Dropdown-Root";
        private const string NAME_HEAD = "GC-UI-Dropdown-Head";
        private const string NAME_BODY = "GC-UI-Dropdown-Body";
        
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement { name = NAME_ROOT };
            VisualElement head = new VisualElement { name = NAME_HEAD };
            VisualElement body = new VisualElement { name = NAME_BODY };
            
            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet sheet in sheets) root.styleSheets.Add(sheet);
            
            root.Add(head);
            root.Add(body);
            
            IMGUIContainer buttonIMGUI = new IMGUIContainer(this.OnInspectorGUI);
            head.Add(buttonIMGUI);

            SerializedProperty fromSource = this.serializedObject.FindProperty("m_SetFromSource");
            SerializedProperty onChangeSet = this.serializedObject.FindProperty("m_OnChangeSet");

            PropertyField fieldFromSource = new PropertyField(fromSource);
            PropertyField fieldOnChangeSet = new PropertyField(onChangeSet);
            
            body.Add(fieldFromSource);
            body.Add(fieldOnChangeSet);

            return root;
        }

        // CREATE: --------------------------------------------------------------------------------

        [MenuItem("GameObject/Game Creator/UI/Dropdown", false, 0)]
        public static void CreateElement()
        {
            GameObject canvas = UnityUIUtilities.GetCanvas();
            
            DefaultControls.Resources resources = UnityUIUtilities.GetStandardResources();
            GameObject dropdownGO = DefaultControls.CreateDropdown(resources);
            dropdownGO.transform.SetParent(canvas.transform, false);
            dropdownGO.layer = UIUtils.LAYER_UI;
            
            Dropdown dropdown = dropdownGO.GetComponent<Dropdown>();
            Graphic targetGraphic = dropdown.targetGraphic;
            RectTransform template = dropdown.template;
            Text captionText = dropdown.captionText;
            UnityEngine.UI.Image captionImage = dropdown.captionImage;
            Text itemText = dropdown.itemText;
            UnityEngine.UI.Image itemImage = dropdown.itemImage;
            List<Dropdown.OptionData> options = dropdown.options;

            DestroyImmediate(dropdown);
            
            dropdown = dropdownGO.AddComponent<DropdownPropertyInteger>();
            dropdown.targetGraphic = targetGraphic;
            dropdown.template = template;
            dropdown.captionText = captionText;
            dropdown.captionImage = captionImage;
            dropdown.itemText = itemText;
            dropdown.itemImage = itemImage;
            dropdown.options = options;
            
            Undo.RegisterCreatedObjectUndo(dropdownGO, $"Create {dropdownGO.name}");
            Selection.activeGameObject = dropdownGO;
        }
    }
}