using GameCreator.Editor.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Installs
{
    [CustomPropertyDrawer(typeof(Dependency))]
    public class DependencyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty id = property.FindPropertyRelative("m_ID");
            SerializedProperty minVersion = property.FindPropertyRelative("m_MinVersion");

            PropertyField fieldID = new PropertyField(id);
            PropertyField fieldMinVersion = new PropertyField(minVersion);

            VisualElement root = new VisualElement();

            root.Add(fieldID);
            root.Add(fieldMinVersion);
            
            return root;
        }
    }
}
