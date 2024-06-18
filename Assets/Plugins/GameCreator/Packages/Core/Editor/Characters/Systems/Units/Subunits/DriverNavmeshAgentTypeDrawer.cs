using System.Collections.Generic;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(DriverNavmeshAgentType))]
    public class DriverNavmeshAgentTypeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            List<int> options = new List<int>();
            int agentsCount = NavMesh.GetSettingsCount();
            
            for (int i = 0; i < agentsCount; ++i)
            {
                options.Add(i);
            }
            
            SerializedProperty agentIndex = property.FindPropertyRelative("m_AgentTypeIndex");

            PopupField<int> popupField = new PopupField<int>(
                property.displayName,
                options, 
                agentIndex.intValue,
                this.IndexToAgentTypeName,
                this.IndexToAgentTypeName
            );

            popupField.RegisterValueChangedCallback(changeEvent =>
            {
                agentIndex.intValue = changeEvent.newValue;
                agentIndex.serializedObject.ApplyModifiedProperties();
                agentIndex.serializedObject.Update();
            });

            popupField.AddToClassList(AlignLabel.CLASS_UNITY_ALIGN_LABEL);
            AlignLabel.On(popupField);
            
            return popupField;
        }

        private string IndexToAgentTypeName(int index)
        {
            NavMeshBuildSettings settings = NavMesh.GetSettingsByIndex(index);
            return NavMesh.GetSettingsNameFromID(settings.agentTypeID);
        }
    }
}