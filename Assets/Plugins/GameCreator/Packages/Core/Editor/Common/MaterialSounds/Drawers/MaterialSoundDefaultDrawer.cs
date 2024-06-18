using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(MaterialSoundDefault))]
    public class MaterialSoundDefaultDrawer : TSectionDrawer
    {
        protected override string Name(SerializedProperty property) => "Default Material Sounds";
        
        protected override void CreatePropertyContent(VisualElement container,
            SerializedProperty property)
        {
            SerializedProperty impact = property.FindPropertyRelative("m_Impact");
            SerializedProperty volume = property.FindPropertyRelative("m_Volume");
            SerializedProperty variations = property.FindPropertyRelative("m_Variations");
        
            container.Add(new LabelTitle("Impact"));
            container.Add(new PropertyField(impact));
            container.Add(new SpaceSmall());
            container.Add(new LabelTitle("Sounds"));
            container.Add(new PropertyField(volume));
            container.Add(new PropertyField(variations));
        }
    }
}