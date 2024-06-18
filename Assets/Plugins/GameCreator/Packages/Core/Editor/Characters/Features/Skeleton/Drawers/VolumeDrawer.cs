using GameCreator.Editor.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    public abstract class VolumeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            SerializedProperty bone = property.FindPropertyRelative("m_Bone");
            SerializedProperty weight = property.FindPropertyRelative("m_Weight");
            SerializedProperty joint = property.FindPropertyRelative("m_Joint");
            
            PropertyField boneField = new PropertyField(bone);
            Slider weightSlider = new Slider(weight.displayName, 0f, 1f)
            {
                bindingPath = weight.propertyPath,
                showInputField = true
            };
            
            weightSlider.AddToClassList(AlignLabel.CLASS_UNITY_ALIGN_LABEL);

            PropertyElement jointSelector = new PropertyElement(joint, joint.displayName, false);
            
            root.Add(boneField);
            root.Add(weightSlider);
            
            root.Add(new SpaceSmall());
            this.CreateGUI(property, root);
            
            root.Add(new SpaceSmall());
            root.Add(jointSelector);

            return root;
        }
        
        // ABSTRACT METHODS: ----------------------------------------------------------------------

        protected abstract void CreateGUI(SerializedProperty property, VisualElement root);
    }
}