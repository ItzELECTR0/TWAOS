using GameCreator.Runtime.Common;
using GameCreator.Runtime.Common.UnityUI;
using TMPro;
using UnityEditor;
using UnityEditor.UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common.UnityUI
{
    [CustomEditor(typeof(ButtonInstructions), true)]
    public class ButtonInstructionsEditor : SelectableEditor
    {
        private const string USS_PATH = EditorPaths.COMMON + "UI/UnityUI/StyleSheets/Button";

        private const string NAME_ROOT = "GC-UI-Button-Root";
        private const string NAME_HEAD = "GC-UI-Button-Head";
        private const string NAME_BODY = "GC-UI-Button-Body";
        
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

            SerializedProperty instructions = this.serializedObject.FindProperty("m_Instructions");
            PropertyField fieldInstructions = new PropertyField(instructions);
            body.Add(fieldInstructions);

            return root;
        }
        
        // CREATION MENU: -------------------------------------------------------------------------
        
        [MenuItem("GameObject/Game Creator/UI/Button", false, 0)]
        public static void CreateElement(MenuCommand menuCommand)
        {
            GameObject canvas = UnityUIUtilities.GetCanvas();
            
            DefaultControls.Resources resources = UnityUIUtilities.GetStandardResources();
            GameObject buttonGO = DefaultControls.CreateButton(resources);
            buttonGO.transform.SetParent(canvas.transform, false);
            buttonGO.layer = UIUtils.LAYER_UI;

            UnityEngine.UI.Button button = buttonGO.GetComponent<UnityEngine.UI.Button>();
            Graphic targetGraphic = button.targetGraphic;

            DestroyImmediate(button);
            
            button = buttonGO.AddComponent<ButtonInstructions>();
            button.targetGraphic = targetGraphic;

            Undo.RegisterCreatedObjectUndo(buttonGO, $"Create {buttonGO.name}");
            Selection.activeGameObject = buttonGO;
        }
        
        [MenuItem("GameObject/Game Creator/UI/Button - TextMeshPro", false, 0)]
        public static void CreateElementTMP(MenuCommand menuCommand)
        {
            GameObject canvas = UnityUIUtilities.GetCanvas();
            
            TMP_DefaultControls.Resources resources = UnityUIUtilities.GetTMPStandardResources();
            GameObject buttonGO = TMP_DefaultControls.CreateButton(resources);
            buttonGO.transform.SetParent(canvas.transform, false);
            buttonGO.layer = UIUtils.LAYER_UI;

            UnityEngine.UI.Button button = buttonGO.GetComponent<UnityEngine.UI.Button>();
            Graphic targetGraphic = button.targetGraphic;

            DestroyImmediate(button);
            
            button = buttonGO.AddComponent<ButtonInstructions>();
            button.targetGraphic = targetGraphic;
            
            Undo.RegisterCreatedObjectUndo(buttonGO, $"Create {buttonGO.name}");
            Selection.activeGameObject = buttonGO;
        }
    }
}