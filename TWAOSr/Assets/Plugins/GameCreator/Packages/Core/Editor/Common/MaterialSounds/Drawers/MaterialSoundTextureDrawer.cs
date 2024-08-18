using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(MaterialSoundTexture))]
    public class MaterialSoundTextureDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            SerializedProperty name = property.FindPropertyRelative("m_Name");
            SerializedProperty texture = property.FindPropertyRelative("m_Texture");
            SerializedProperty volume = property.FindPropertyRelative("m_Volume");
            SerializedProperty impact = property.FindPropertyRelative("m_Impact");
            SerializedProperty variations = property.FindPropertyRelative("m_Variations");
            
            root.Add(new PropertyField(name));
            root.Add(new PropertyField(texture));
            root.Add(new SpaceSmaller());
            root.Add(new LabelTitle("Impact"));
            root.Add(new PropertyField(impact));
            root.Add(new SpaceSmaller());
            root.Add(new LabelTitle("Sounds"));
            root.Add(new PropertyField(volume));
            root.Add(new PropertyField(variations));
            
            return root;
        }
    }
}