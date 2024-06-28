using DaimahouGames.Editor.Common;
using DaimahouGames.Editor.Core;
using DaimahouGames.Runtime.Abilities;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DaimahouGames.Editor.Abilities
{
    [CustomEditor(typeof(Impact), true)]
    public class ImpactInspector: UnityEditor.Editor
    {
        //============================================================================================================||
        // ※  Variables: -------------------------------------------------------------------------------------------|※
        // ---|　Exposed State ----------------------------------------------------------------------------------->|
        // ---|　Internal State ---------------------------------------------------------------------------------->|
        
        private VisualElement m_Root;

        private FoldoutInspector m_Settings;
        
        // ---|　Dependencies ------------------------------------------------------------------------------------>|
        // ---|　Properties -------------------------------------------------------------------------------------->|
        // ---|　Events ------------------------------------------------------------------------------------------>|
        //============================================================================================================||
        // ※  Constructors: ----------------------------------------------------------------------------------------|※
        // ※  Initialization Methods: ------------------------------------------------------------------------------|※
        // ※  Public Methods: --------------------------------------------------------------------------------------|※

        public override VisualElement CreateInspectorGUI()
        {
            m_Root = new VisualElement();
            
            var prefabProperty = serializedObject.FindProperty("m_Prefab");
            var effectsProperty = serializedObject.FindProperty("m_Effects");
            var filtersProperty = serializedObject.FindProperty("m_Filters");
            var delayProperty = serializedObject.FindProperty("m_Delay");
            var radiusProperty = serializedObject.FindProperty("m_Radius");
            var layerProperty = serializedObject.FindProperty("m_TargetLayer");

            var prefabInspector = new PropertyField(prefabProperty);
            
            var effectsInspector = new GenericListInspector<AbilityEffect>(effectsProperty);
            var filtersInspector = new GenericListInspector<AbilityFilter>(filtersProperty);

            m_Settings = new FoldoutInspector();
            m_Root.Add(new SpaceSmall());
            m_Root.Add(m_Settings);
            
            var delayInspector = new PropertyField(delayProperty);
            var radiusInspector = new PropertyField(radiusProperty);
            var layerInspector = new PropertyField(layerProperty);
            
            m_Root.Add(new SpaceSmall());
            filtersInspector.SetTitle("Filters");
            m_Root.Add(filtersInspector);
            m_Root.Add(new SpaceSmall());
            
            m_Root.Add(new SpaceSmall());
            effectsInspector.SetTitle("Effects");
            m_Root.Add(effectsInspector);
            
            m_Settings.AddBodyElement(prefabInspector);
            m_Settings.AddBodyElement(new SpaceSmall());
            m_Settings.AddBodyElement(radiusInspector);
            m_Settings.AddBodyElement(layerInspector);
            m_Settings.AddBodyElement(new SpaceSmall());
            m_Settings.AddBodyElement(delayInspector);
            
            m_Settings.SetIcon(null);
            SetSettingsTitle();
            
            prefabInspector.RegisterValueChangeCallback(_ => SetSettingsTitle());
            radiusInspector.RegisterValueChangeCallback(_ => SetSettingsTitle());
            delayInspector.RegisterValueChangeCallback(_ => SetSettingsTitle());
            
            return m_Root;
        }

        private void SetSettingsTitle()
        {
            var prefabProperty = serializedObject.FindProperty("m_Prefab")?.GetSerializedValue<PropertyGetGameObject>();
            
            var prefabName = prefabProperty != null
                ? prefabProperty.ToString()
                : "(none)";
            
            var delayProperty = serializedObject.FindProperty("m_Delay").floatValue;
            var radiusProperty = serializedObject.FindProperty("m_Radius").GetSerializedValue<PropertyGetDecimal>();

            var radius = radiusProperty.Get(Args.EMPTY) > 0 
                ? $" [{radiusProperty.Get(Args.EMPTY)}m radius]" 
                : "";

            var delay = delayProperty > 0
                ? $" [{delayProperty}s delay]"
                : "";

            m_Settings.SetTile($"Impact Settings: {prefabName}{radius}{delay}");
        }
        
        // ※  Virtual Methods: -------------------------------------------------------------------------------------|※
        // ※  Protected Methods: -----------------------------------------------------------------------------------|※
        // ※  Private Methods: -------------------------------------------------------------------------------------|※
        //============================================================================================================||
    }
}