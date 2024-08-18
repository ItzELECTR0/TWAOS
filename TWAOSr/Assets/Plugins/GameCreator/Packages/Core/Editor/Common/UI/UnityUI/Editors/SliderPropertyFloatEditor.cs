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
    [CustomEditor(typeof(SliderPropertyFloat))]
    public class SliderPropertyFloatEditor : SliderEditor
    {
        private const string USS_PATH = EditorPaths.COMMON + "UI/UnityUI/StyleSheets/Slider";

        private const string NAME_ROOT = "GC-UI-Slider-Root";
        private const string NAME_HEAD = "GC-UI-Slider-Head";
        private const string NAME_BODY = "GC-UI-Slider-Body";
        
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

        [MenuItem("GameObject/Game Creator/UI/Slider", false, 0)]
        public static void CreateElement()
        {
            GameObject canvas = UnityUIUtilities.GetCanvas();
            
            DefaultControls.Resources resources = UnityUIUtilities.GetStandardResources();
            GameObject sliderGO = DefaultControls.CreateSlider(resources);
            sliderGO.transform.SetParent(canvas.transform, false);
            sliderGO.layer = UIUtils.LAYER_UI;

            UnityEngine.UI.Slider slider = sliderGO.GetComponent<UnityEngine.UI.Slider>();
            Graphic targetGraphic = slider.targetGraphic;
            RectTransform rectFill = slider.fillRect;
            RectTransform rectHandle = slider.handleRect;

            DestroyImmediate(slider);
            
            slider = sliderGO.AddComponent<SliderPropertyFloat>();
            slider.targetGraphic = targetGraphic;
            slider.fillRect = rectFill;
            slider.handleRect = rectHandle;
            
            Undo.RegisterCreatedObjectUndo(sliderGO, $"Create {sliderGO.name}");
            Selection.activeGameObject = sliderGO;
        }
    }
}