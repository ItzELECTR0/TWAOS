using GameCreator.Editor.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.VisualScripting
{
    [CustomPropertyDrawer(typeof(InstructionCharacterNavigationMoveTo.NavigationOptions))]
    public class NavigationOptionsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            VisualElement head = new VisualElement();
            VisualElement body = new VisualElement();
            
            root.Add(head);
            root.Add(body);
            
            SerializedProperty waitToArrive = property.FindPropertyRelative("m_WaitToArrive");
            SerializedProperty cancelOnFail = property.FindPropertyRelative("m_CancelOnFail");
            SerializedProperty onFail = property.FindPropertyRelative("m_OnFail");

            PropertyField fieldWaitToArrive = new PropertyField(waitToArrive);
            head.Add(fieldWaitToArrive);
            
            PropertyField fieldCancelOnFail = new PropertyField(cancelOnFail);
            PropertyField fieldOnFail = new PropertyField(onFail);
            
            body.Add(fieldCancelOnFail);
            body.Add(new LabelTitle("On Fail:"));
            body.Add(new SpaceSmallest());
            body.Add(fieldOnFail);
        
            fieldWaitToArrive.RegisterValueChangeCallback(changeEvent =>
            {
                body.style.display = changeEvent.changedProperty.boolValue
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            });
            
            body.style.display = waitToArrive.boolValue
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            return root;
        }
    }
}
