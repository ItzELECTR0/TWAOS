using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferenceDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            SerializedProperty propertyAsset = property.FindPropertyRelative("m_Scene");
            
            ObjectField fieldAsset = new ObjectField(propertyAsset.displayName)
            {
                allowSceneObjects = false,
                objectType = typeof(SceneAsset),
                bindingPath = propertyAsset.propertyPath
            };

            AlignLabel.On(fieldAsset);
            root.Add(fieldAsset);

            return root;
        }
    }
}