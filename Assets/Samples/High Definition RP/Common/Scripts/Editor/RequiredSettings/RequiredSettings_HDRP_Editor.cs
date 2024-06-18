using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEditor;
using System;

namespace UnityEditor.Rendering.HighDefinition
{
    [CustomPropertyDrawer(typeof(HDRPSettingsSectionAttribute))]
    public class HDRPSettingsSectionDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            HDRPSettingsSectionAttribute settingsSection = attribute as HDRPSettingsSectionAttribute;

            if (property.propertyType == SerializedPropertyType.Integer)
            {
                if (settingsSection.rootSection != null)
                {
                    var so = property.serializedObject;
                    var fullPath = new List<string>( property.propertyPath.Split("."[0]) );
                    fullPath[fullPath.Count - 1] = settingsSection.rootSection;
                    SerializedProperty rootSectionProp = null;

                    rootSectionProp = so.FindProperty(fullPath[0]);
                    if (rootSectionProp == null)
                        return;

                    fullPath.RemoveAt(0);

                    while (fullPath.Count > 0)
                    {
                        rootSectionProp = rootSectionProp.FindPropertyRelative(fullPath[0]);
                        if (rootSectionProp == null)
                            return;
                        fullPath.RemoveAt(0);
                    }

                    HDRenderPipelineUI.ExpandableGroup rootSection = (HDRenderPipelineUI.ExpandableGroup) rootSectionProp.intValue;
                    switch (rootSection)
                    {
                        case HDRenderPipelineUI.ExpandableGroup.Lighting:
                            property.intValue = (int)(HDRenderPipelineUI.ExpandableLighting)EditorGUI.EnumPopup(position, label, (HDRenderPipelineUI.ExpandableLighting)property.intValue);
                            break;
                        case HDRenderPipelineUI.ExpandableGroup.LightingTiers:
                            property.intValue = (int)(HDRenderPipelineUI.ExpandableLightingQuality)EditorGUI.EnumPopup(position, label, (HDRenderPipelineUI.ExpandableLightingQuality)property.intValue);
                            break;
                        case HDRenderPipelineUI.ExpandableGroup.PostProcess:
                            property.intValue = (int)(HDRenderPipelineUI.ExpandablePostProcess)EditorGUI.EnumPopup(position, label, (HDRenderPipelineUI.ExpandablePostProcess)property.intValue);
                            break;
                        case HDRenderPipelineUI.ExpandableGroup.PostProcessTiers:
                            property.intValue = (int)(HDRenderPipelineUI.ExpandablePostProcessQuality)EditorGUI.EnumPopup(position, label, (HDRenderPipelineUI.ExpandablePostProcessQuality)property.intValue);
                            break;
                        default:
                            property.intValue = (int)(HDRenderPipelineUI.ExpandableRendering)EditorGUI.EnumPopup(position, label, (HDRenderPipelineUI.ExpandableRendering)property.intValue);
                            break;
                    }
                }
                else
                {
                    property.intValue = (int)(HDRenderPipelineUI.ExpandableGroup)EditorGUI.EnumPopup(position, label, (HDRenderPipelineUI.ExpandableGroup)property.intValue);
                }
            }
            else
                EditorGUI.LabelField(position, label.text, "Use HDRPSettingsSection with int.");
        }
    }

    public class HDRPRequiredSettings_Editor
    {
        public static void ShowSetting(RequiredSettingHDRP setting)
        {
            SettingsService.OpenProjectSettings(setting.projectSettingsPath);
            // HDRenderPipelineUI.Inspector.Expand(setting.uiSectionInt);
            // HDRenderPipelineUI.Inspector.Expand(setting.uiSubSectionInt);
            HDRenderPipelineUI.SubInspectors[(HDRenderPipelineUI.ExpandableGroup)setting.uiSectionInt].Expand(setting.uiSubSectionInt);
            CoreEditorUtils.Highlight("Project Settings", setting.propertyPath, HighlightSearchMode.Identifier);
        }
    }
}
