using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using GameCreator.Runtime.Common;
using UnityEngine.InputSystem;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(InputActionFromAsset), true)]
    public class InputActionFromAssetDrawer : PropertyDrawer
    {
        private const string USS_PATH = EditorPaths.COMMON + "Input/StyleSheets/InputActionFromAsset";
        private const string NAME_CUSTOM_BODY = "GC-Input-Custom-Body";
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            VisualElement head = new VisualElement();
            VisualElement body = new VisualElement { name = NAME_CUSTOM_BODY };

            root.Add(head);
            root.Add(body);

            StyleSheet[] styleSheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in styleSheets)
            {
                root.styleSheets.Add(styleSheet);
            }
            
            SerializedProperty inputAsset = property.FindPropertyRelative("m_InputAsset");
            PropertyField fieldInputAsset = new PropertyField(inputAsset);

            head.Add(fieldInputAsset);

            this.RefreshCustomBody(property, body);
            fieldInputAsset.RegisterValueChangeCallback(_ =>
            {
                this.RefreshCustomBody(property, body);
            });
        
            return root;
        }

        private void RefreshCustomBody(SerializedProperty property, VisualElement body)
        {
            body.Clear();

            SerializedProperty propInputAsset = property.FindPropertyRelative("m_InputAsset");
            SerializedProperty propActionMap = property.FindPropertyRelative("m_ActionMap");
            SerializedProperty propAction = property.FindPropertyRelative("m_Action");

            string currentAction = $"{propActionMap.stringValue}/{propAction.stringValue}";
            int currentIndex = 0;

            List<string> options = new List<string> { string.Empty };
            InputActionAsset inputAsset = propInputAsset.objectReferenceValue as InputActionAsset;
            
            if (inputAsset != null)
            {
                foreach (InputActionMap inputActionMap in inputAsset.actionMaps)
                {
                    foreach (InputAction inputAction in inputActionMap.actions)
                    {
                        string option = $"{inputActionMap.name}/{inputAction.name}";
                        if (currentAction == option) currentIndex = options.Count;
                        options.Add(option);
                    }
                }
            }
            
            PopupField<string> popupField = new PopupField<string>(
                " ",
                options, 
                currentIndex,
                option => option,
                option => option
            );

            body.Add(popupField);
            body.SetEnabled(options.Count > 1);

            popupField.RegisterValueChangedCallback(changeEvent =>
            {
                property.serializedObject.Update();
                
                string[] split = changeEvent.newValue.Split('/');
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
                
                property.serializedObject.ApplyModifiedProperties();
            });
            
            popupField.AddToClassList(AlignLabel.CLASS_UNITY_ALIGN_LABEL);
            AlignLabel.On(popupField);
        }
    }
}