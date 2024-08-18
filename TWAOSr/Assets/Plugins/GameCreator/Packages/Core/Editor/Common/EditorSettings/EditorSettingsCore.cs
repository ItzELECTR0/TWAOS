using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public static class EditorSettingsCore
    {
        private const string LABEL = "Core";

        // PROPERTIES: ----------------------------------------------------------------------------

        private static float MarkerCapsuleHeight
        {
            get => EditorPrefs.GetFloat(Marker.KEY_MARKER_CAPSULE_HEIGHT, 2f);
            set => EditorPrefs.SetFloat(Marker.KEY_MARKER_CAPSULE_HEIGHT, value);
        }
        
        private static float MarkerCapsuleRadius
        {
            get => EditorPrefs.GetFloat(Marker.KEY_MARKER_CAPSULE_RADIUS, 0.2f);
            set => EditorPrefs.SetFloat(Marker.KEY_MARKER_CAPSULE_RADIUS, value);
        }

        private static bool RunnersShowRunnerContainer
        {
            get => EditorPrefs.GetBool(Runner.KEY_RUNNER_SHOW_CONTAINER, false);
            set => EditorPrefs.SetBool(Runner.KEY_RUNNER_SHOW_CONTAINER, value);
        }
        
        // REGISTRATION METHODS: ------------------------------------------------------------------
        
        [SettingsProvider]
        private static SettingsProvider CreateEditorSettingsCore()
        {
            return EditorSettingsRegistrar.CreateSettings(LABEL, CreateContent);
        }

        [InitializeOnLoadMethod]
        private static void RegisterSettings()
        {
            EditorSettingsRegistrar.RegisterSettings(LABEL);
        }
        
        // CONTENT: -------------------------------------------------------------------------------

        private static void CreateContent(string search, VisualElement content)
        {
            content.style.paddingLeft = new Length(10, LengthUnit.Pixel);
            content.style.paddingRight = new Length(5, LengthUnit.Pixel);
            
            content.Add(new SpaceSmall());
            content.Add(new LabelTitle("Marker Gizmos:"));
            
            FloatField fieldHeight = new FloatField("Height") { value = MarkerCapsuleHeight };
            FloatField fieldRadius = new FloatField("Radius") { value = MarkerCapsuleRadius };

            fieldHeight.RegisterValueChangedCallback(changeEvent => MarkerCapsuleHeight = changeEvent.newValue);
            fieldRadius.RegisterValueChangedCallback(changeEvent => MarkerCapsuleRadius = changeEvent.newValue);
            
            content.Add(fieldHeight);
            content.Add(fieldRadius);
            
            content.Add(new SpaceSmall());
            content.Add(new LabelTitle("Runners:"));
            
            Toggle fieldRunnerShow = new Toggle("Show Runners") { value = RunnersShowRunnerContainer };
            fieldRunnerShow.RegisterValueChangedCallback(changeEvent => RunnersShowRunnerContainer = changeEvent.newValue);
            
            content.Add(fieldRunnerShow);
        }
    }
}