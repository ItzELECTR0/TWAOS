using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using GameCreator.Runtime.VisualScripting;

namespace GameCreator.Editor.VisualScripting
{
    [CustomPropertyDrawer(typeof(Branch))]
    public class BranchDrawer : PropertyDrawer
    {
        private static readonly IIcon ICON_CONDITION = new IconCondition(ColorTheme.Type.Green);
        private static readonly IIcon ICON_INSTRUCTIONS = new IconInstructions(ColorTheme.Type.Blue);
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new VisualElement();
            
            SerializedProperty propertyConditions = property.FindPropertyRelative("m_ConditionList");
            SerializedProperty propertyInstructions = property.FindPropertyRelative("m_InstructionList");

            PropertyField fieldConditions = new PropertyField(propertyConditions);
            PropertyField fieldInstructions = new PropertyField(propertyInstructions);
            
            container.Add(this.CreateLabel("If", ICON_CONDITION));
            container.Add(fieldConditions);
            
            container.Add(new SpaceSmall());
            container.Add(this.CreateLabel("Then", ICON_INSTRUCTIONS));
            container.Add(fieldInstructions);

            return container;
        }

        private VisualElement CreateLabel(string text, IIcon icon)
        {
            VisualElement element = new VisualElement();
            element.AddToClassList("gc-branch-label");

            element.Add(new Image { image = icon.Texture });
            element.Add(new Label(text));

            return element;
        }
    }
}