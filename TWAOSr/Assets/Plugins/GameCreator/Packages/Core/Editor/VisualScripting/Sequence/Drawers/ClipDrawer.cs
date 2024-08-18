using GameCreator.Editor.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.VisualScripting
{
    [CustomPropertyDrawer(typeof(Clip), true)]
    public class ClipDrawer : PropertyDrawer
    {
        public const string PROP_TIME = "m_Time";
        public const string PROP_DURATION = "m_Duration";
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            SerializationUtils.CreateChildProperties(
                root, property, SerializationUtils.ChildrenMode.ShowLabelsInChildren, true, 
                PROP_TIME, PROP_DURATION
            );
            
            root.Bind(property.serializedObject);
            return root;
        }
    }
}