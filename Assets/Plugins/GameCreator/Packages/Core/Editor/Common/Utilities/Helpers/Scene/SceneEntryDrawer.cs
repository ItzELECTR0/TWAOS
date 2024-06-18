using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(SceneEntry))]
    public class SceneEntryDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement content = new VisualElement();

            SerializedProperty gameObject = property.FindPropertyRelative("m_Target");
            SerializedProperty location = property.FindPropertyRelative("m_Location");

            PropertyField fieldGameObject = new PropertyField(gameObject);
            PropertyField fieldLocation = new PropertyField(location);
            
            content.Add(fieldGameObject);
            content.Add(fieldLocation);
            
            return content;
        }
    }
}
