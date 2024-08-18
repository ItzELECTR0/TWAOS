using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public abstract class TPercentageDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty value = property.FindPropertyRelative("m_Value"); 
            
            Slider fieldSlider = new Slider(0f, 1f)
            {
                label = this.GetLabel(property), 
                bindingPath = value.propertyPath
            };

            FloatField fieldValue = new FloatField
            {
                label = string.Empty,
                isDelayed = true, 
                bindingPath = value.propertyPath
            };

            fieldValue.style.marginLeft = 5;
            fieldValue.style.width = 80;
            
            fieldValue.RegisterValueChangedCallback(changeEvent =>
            {
                fieldSlider.value = changeEvent.newValue;
            });
            
            fieldSlider.Add(fieldValue);
            return fieldSlider;
        }

        protected abstract string GetLabel(SerializedProperty property);
    }
}