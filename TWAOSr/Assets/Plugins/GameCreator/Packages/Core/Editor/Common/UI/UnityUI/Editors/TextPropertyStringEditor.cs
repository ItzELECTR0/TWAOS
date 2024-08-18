using GameCreator.Runtime.Common;
using GameCreator.Runtime.Common.UnityUI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common.UnityUI
{
    [CustomEditor(typeof(TextPropertyString))]
    public class TextPropertyStringEditor : UnityEditor.UI.TextEditor
    {
        private const string USS_PATH = EditorPaths.COMMON + "UI/UnityUI/StyleSheets/Text";

        private const string NAME_ROOT = "GC-UI-Text-Root";
        private const string NAME_HEAD = "GC-UI-Text-Head";
        private const string NAME_BODY = "GC-UI-Text-Body";
        
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement { name = NAME_ROOT };
            VisualElement head = new VisualElement { name = NAME_HEAD };
            VisualElement body = new VisualElement { name = NAME_BODY };
            
            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet sheet in sheets) root.styleSheets.Add(sheet);
            
            root.Add(head);
            root.Add(body);
            
            IMGUIContainer textIMGUI = new IMGUIContainer(this.OnInspectorGUI);
            head.Add(textIMGUI);

            SerializedProperty value = this.serializedObject.FindProperty("m_Value");
            PropertyField fieldFromSource = new PropertyField(value);

            body.Add(fieldFromSource);

            return root;
        }

        // CREATE: --------------------------------------------------------------------------------

        [MenuItem("GameObject/Game Creator/UI/Text", false, 0)]
        public static void CreateElement()
        {
            GameObject canvas = UnityUIUtilities.GetCanvas();
            
            DefaultControls.Resources resources = UnityUIUtilities.GetStandardResources();
            GameObject textGO = DefaultControls.CreateText(resources);
            textGO.transform.SetParent(canvas.transform, false);
            textGO.layer = UIUtils.LAYER_UI;

            Text text = textGO.GetComponent<Text>();
            Font targetFont = text.font;

            DestroyImmediate(text);
            
            text = textGO.AddComponent<TextPropertyString>();
            text.font = targetFont;

            Undo.RegisterCreatedObjectUndo(textGO, $"Create {textGO.name}");
            Selection.activeGameObject = textGO;
        }
    }
}