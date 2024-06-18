using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(MaterialSoundsData))]
    public class MaterialSoundsDataDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            SerializedProperty layerMask = property.FindPropertyRelative("m_LayerMask");

            MaterialSoundsTool materialSoundsTool = new MaterialSoundsTool(property);
            SerializedProperty defaultSounds = property.FindPropertyRelative("m_DefaultSounds");
            
            root.Add(new PropertyField(layerMask));
            root.Add(materialSoundsTool);
            root.Add(new PropertyField(defaultSounds));

            return root;
        }
    }
}