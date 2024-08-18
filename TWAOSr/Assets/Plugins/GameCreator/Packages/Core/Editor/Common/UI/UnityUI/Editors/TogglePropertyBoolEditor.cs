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
    [CustomEditor(typeof(TogglePropertyBool))]
    public class TogglePropertyBoolEditor : ToggleEditor
    {
        private const string USS_PATH = EditorPaths.COMMON + "UI/UnityUI/StyleSheets/Toggle";

        private const string NAME_ROOT = "GC-UI-Toggle-Root";
        private const string NAME_HEAD = "GC-UI-Toggle-Head";
        private const string NAME_BODY = "GC-UI-Toggle-Body";
        
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

        [MenuItem("GameObject/Game Creator/UI/Toggle", false, 0)]
        public static void CreateElement()
        {
            GameObject canvas = UnityUIUtilities.GetCanvas();
            
            DefaultControls.Resources resources = UnityUIUtilities.GetStandardResources();
            GameObject toggleGO = DefaultControls.CreateToggle(resources);
            toggleGO.transform.SetParent(canvas.transform, false);
            toggleGO.layer = UIUtils.LAYER_UI;

            UnityEngine.UI.Toggle toggle = toggleGO.GetComponent<UnityEngine.UI.Toggle>();
            Graphic targetGraphic = toggle.targetGraphic;
            Graphic graphic = toggle.graphic;

            DestroyImmediate(toggle);
            
            toggle = toggleGO.AddComponent<TogglePropertyBool>();
            toggle.targetGraphic = targetGraphic;
            toggle.graphic = graphic;
            
            Undo.RegisterCreatedObjectUndo(toggleGO, $"Create {toggleGO.name}");
            Selection.activeGameObject = toggleGO;
        }
    }
}