using System.Collections.Generic;
using System.Linq;
using GameCreator.Editor.Common;
using DaimahouGames.Editor.Core;
using UnityEditor.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace DaimahouGames.Editor.Core
{
    public class InputMappingInspector : GenericInspector
    {
        public InputMappingInspector(ListInspector listInspector, int index) : base(listInspector, index) {}

        protected override void SetupBody()
        {
            var propInputAsset = ListInspector.PropertyList.serializedObject.FindProperty("m_InputAsset");
            var propActionMap = m_Property.FindPropertyRelative("ActionMap");
            var propAction = m_Property.FindPropertyRelative("Action");

            var currentAction = $"{propActionMap.stringValue}/{propAction.stringValue}";
            var currentIndex = 0;

            var options = new List<string> { string.Empty };
            var inputAsset = propInputAsset.objectReferenceValue as InputActionAsset;
            
            if (inputAsset != null)
            {
                var inputMappings = 
                    from inputActionMap in inputAsset.actionMaps
                    from inputAction in inputActionMap.actions
                    select $"{inputActionMap.name}/{inputAction.name}";
                        
                foreach (var option in inputMappings)
                {
                    if (currentAction == option) currentIndex = options.Count;
                    options.Add(option);
                }
            }
            
            var popupField = new PopupField<string>(
                " ",
                options, 
                currentIndex,
                option => option,
                option => option
            );

            m_Body.Add(popupField);
            m_Body.SetEnabled(options.Count > 1);

            popupField.RegisterValueChangedCallback(changeEvent =>
            {
                m_Property.serializedObject.Update();
                
                var split = changeEvent.newValue.Split('/');
                if (split.Length != 2)
                {
                    propActionMap.stringValue = "";
                    propAction.stringValue = "";
                }
                else
                {
                    propActionMap.stringValue = split[0];
                    propAction.stringValue = split[1];
                }
                
                m_Property.serializedObject.ApplyModifiedProperties();
                UpdateHead();
            });
            
            SerializationUtils.CreateChildProperties(
                m_Body,
                m_Property,
                SerializationUtils.ChildrenMode.ShowLabelsInChildren,
                false, 
                "ActionMap", "Action");
            
            UpdateBody();
        }
    }
}