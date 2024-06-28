using DaimahouGames.Editor.Common;
using DaimahouGames.Editor.Core;
using DaimahouGames.Runtime.Abilities;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using IconAimTarget = DaimahouGames.Runtime.Abilities.IconAimTarget;

namespace DaimahouGames.Editor.Abilities
{
    [CustomEditor(typeof(Projectile), true)]
    public class ProjectileInspector: UnityEditor.Editor
    {
        private VisualElement m_Root;
        private FoldoutInspector m_PrefabSettings;
        private FoldoutInspector m_MovementSettings;
        private FoldoutInspector m_DeviationSettings;
        private FoldoutInspector m_HomingSettings;

        public override VisualElement CreateInspectorGUI()
        {
            m_Root = new VisualElement();

            m_PrefabSettings = new FoldoutInspector();
            SetPrefabSettingsTitle();
            m_PrefabSettings.SetIcon(new IconCubeSolid(ColorTheme.Type.Blue));
            m_PrefabSettings.AddBodyElements(serializedObject, "m_Prefab");
            
            m_MovementSettings = new FoldoutInspector();
            m_MovementSettings.SetTile("Movement Settings");
            m_MovementSettings.SetIcon(new IconArrowRight(ColorTheme.Type.Blue));
            m_MovementSettings.AddBodyElements(serializedObject, 
                "m_Acceleration",
                "m_Speed", 
                "m_ProjectileRange", 
                "m_PierceTarget", 
                "m_AlwaysExplode",  
                "m_ConstantRange",
                "m_FixedHeight"
            );
            m_MovementSettings.IsExpanded = true;
            m_MovementSettings.Refresh();
            
            m_DeviationSettings = new FoldoutInspector();
            SetDeviationTitle();
            
            m_HomingSettings = new FoldoutInspector();
            SetHomingTitle();
            
            m_Root.Add(new SpaceSmall());
            m_Root.Add(m_PrefabSettings);
            m_Root.Add(m_MovementSettings);
            m_Root.Add(m_DeviationSettings);
            m_Root.Add(m_HomingSettings);
            
            var filtersProperty = serializedObject.FindProperty("m_Filters");
            var filtersInspector = new GenericListInspector<AbilityFilter>(filtersProperty);
            
            var effectsProperty = serializedObject.FindProperty("m_Effects");
            var effectsInspector = new GenericListInspector<AbilityEffect>(effectsProperty);

            var homingField = new PropertyField(serializedObject.FindProperty("m_Homing"));
            var homingPrecisionField = new PropertyField(serializedObject.FindProperty("m_HomingPrecision"));
            homingField.RegisterValueChangeCallback(changed =>
            {
                SetHomingTitle();
            });
            
            homingPrecisionField.RegisterValueChangeCallback(changed =>
            {
                SetHomingTitle();
            });

            var enableDeviationField = new PropertyField(serializedObject.FindProperty("m_EnableDeviation"));
            enableDeviationField.RegisterValueChangeCallback(changed =>
            {
                SetDeviationTitle();
            });
            
            m_HomingSettings.AddBodyElement(homingField);
            m_HomingSettings.AddBodyElement(homingPrecisionField);
            m_HomingSettings.SetIcon(new IconAimTarget(ColorTheme.Type.Blue));
            
            m_DeviationSettings.AddBodyElement(enableDeviationField);
            m_DeviationSettings.AddBodyElements(serializedObject, 
                "m_Frequency",
                "m_RandomizeDeviation", 
                "m_LoopDeviation", "m_ConstantDeviation", 
                "m_HorizontalDeviation", "m_VerticalDeviation", 
                "m_BackwardDeviation", "m_DeviationMultiplier"
            );
            m_DeviationSettings.SetIcon(new IconBranch(ColorTheme.Type.Blue));
            
            m_Root.Add(new SpaceSmall());
            filtersInspector.SetTitle("Filters");
            m_Root.Add(filtersInspector);
            
            m_Root.Add(new SpaceSmall());
            effectsInspector.SetTitle("Effects");
            m_Root.Add(effectsInspector);
            
            return m_Root;
        }

        private void SetPrefabSettingsTitle()
        {
            var prefabProperty = serializedObject.FindProperty("m_Prefab")?.GetSerializedValue<PropertyGetGameObject>();
            
            var prefabName = prefabProperty != null
                ? prefabProperty.ToString()
                : "(none)";

            m_PrefabSettings.SetTile($"Prefab: {prefabName}");
        }

        private void SetHomingTitle()
        {
            var homingField = serializedObject.FindProperty("m_Homing");
            var precisionField = serializedObject.FindProperty("m_HomingPrecision");
            
            var homing = homingField.GetSerializedValue<bool>();
            var latency = precisionField.GetSerializedValue<float>();

            m_HomingSettings.SetTile(string.Format("Homing Settings{0}{1}", homing 
                    ? " - Activated" 
                    : "", homing && latency > 0 ? $" [Precision: {latency}]" : "")
            );
        }

        private void SetDeviationTitle()
        {
            var enableDeviationProperty = serializedObject.FindProperty("m_EnableDeviation");
            m_DeviationSettings.SetTile(enableDeviationProperty.boolValue
                ? "Deviation Settings - ON"
                : "Deviation Settings - OFF"
            );
        }
    }
}